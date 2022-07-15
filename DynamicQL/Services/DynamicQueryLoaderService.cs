using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using DynamicQL.Attributes;
using DynamicQL.Extensions;
using DynamicQL.Model;
using DynamicQL.Model.Graphs;
using DynamicQL.Model.Types;
using DynamicQL.Utility;
using GraphQL;
using GraphQL.Language.AST;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace DynamicQL.Services;

public class DynamicQueryLoaderService : IDynamicQueryLoaderService
{
    private static Dictionary<(Type, string), Expression> _readyToGoExpressions = new ();

    private readonly MethodInfo _runIncludeGenericMethodDefinition = typeof(DynamicQueryLoaderService)
        .GetMethod(nameof(RunInclude))!
        .GetGenericMethodDefinition();
    
    private readonly MethodInfo _runThenIncludeGenericMethodDefinition = typeof(DynamicQueryLoaderService)
        .GetMethod(nameof(RunThenInclude))!
        .GetGenericMethodDefinition();

    private readonly IncludeFunctionTreeGraph _includeFunctionTreeGraph;
    private readonly DynamicQLOptions _options;
    
    public DynamicQueryLoaderService(IncludeFunctionTreeGraph treeGraph, DynamicQLOptions options)
    {
        _includeFunctionTreeGraph = treeGraph;
        _options = options;
    }

    public IQueryable<T> LoadDynamicQueries<T>(
        DbContext dbContext,
        IResolveFieldContext fieldContext
        )
        where T : class
    {
        return LoadDynamicQueries(dbContext.Set<T>(), fieldContext);
    }

    private IQueryable<T> LoadDynamicQueries<T>(
        IQueryable<T> query,
        IResolveFieldContext fieldContext
        )
        where T : class
    {
        throw new NotImplementedException("Planed for update");
        
        var t = typeof(T);
        
        if (!StaticData.TypeToGraphQLTypeMetaInfoMap.TryGetValue(t, out var queryTypeInfo))
        {
            return query;
        }
        
        // Type -> resolvedType
        // bool -> isEnumerable
        var resolvedTypes = new List<Type>() { typeof(T) };
        
        var topNode = fieldContext.Document.Children
            .SelectMany(x => x.Children!)
            .SelectMany(x => x.Children!)
            .Cast<Field>()
            .FirstOrDefault(x => x.Name == queryTypeInfo.SingleGraphName
                                 || x.Name == queryTypeInfo.MultiGraphName);

        if (topNode == null)
            return query;

        var subQueries = topNode.SelectionSet.Selections
            .Where(x => ((Field)x).SelectionSet?.Selections?.Count != 0)
            .Cast<Field>()
            .ToList();

        var nodesToGo = subQueries;
        var nodesIndex = 0;
        var currentNode = nodesToGo[nodesIndex];
        while (currentNode?.SelectionSet?.Selections.Count != 0)
        {
            var subSubQueries = topNode.SelectionSet.Selections
                .Where(x => ((Field)x).SelectionSet?.Selections?.Count != 0)
                .Cast<Field>();
            
            nodesToGo.AddRange(subSubQueries);
            
            nodesIndex++;
            currentNode = nodesToGo[nodesIndex];
        }

    }

    public IQueryable<T> LoadDynamicQueries<T>(DbContext dbContext, 
        IResolveFieldContext fieldContext, 
        bool? isMultiple = null) 
        where T : class
    {
        var t = typeof(T);
        
        if (!StaticData.TypeToGraphQLTypeMetaInfoMap.TryGetValue(t, out var queryTypeInfo))
        {
            return dbContext.Set<T>();
        }
        
        var query = dbContext.Set<T>().AsNoTracking();

        if (_options.MaxQueryDepth == 0)
            return query;
            
        // Type -> resolvedType
        // bool -> isEnumerable
        var resolvedTypes = new List<Type>() { typeof(T) };

        var topNode = fieldContext.Document.Children
            .SelectMany(x => x.Children!)
            .Where(x => x is SelectionSet)
            .SelectMany(x => x.Children!)
            .Cast<Field>()
            .FirstOrDefault(x => x.Name == queryTypeInfo.SingleGraphName
                                 || x.Name == queryTypeInfo.MultiGraphName);

        if (topNode == null)
            return query;
        
        var subQueries = topNode.SelectionSet.Selections
            .Where(x => ((Field)x).SelectionSet?.Selections?.Count != 0)
            .ToList();
        
        foreach (var subQuery in subQueries.Cast<Field>())
        {
            if (!queryTypeInfo.SubQueries.TryGetValue(subQuery.Name, out var queryFieldInfo)) continue;
            
            // methodInfo -> includeQuery method
            // string -> propertyName
            // Type -> BaseType
            // bool -> IsEnumerable
            var functionTree = new List<(MethodInfo, string, Type, bool)>
            {
                (
                    _runIncludeGenericMethodDefinition.MakeGenericMethod(t, queryFieldInfo.PropertyInfo.PropertyType), 
                    queryFieldInfo.Name,
                    queryFieldInfo.BaseType,
                    queryFieldInfo.IsEnumerable
                )
            };

            var subSubQueries = subQuery.SelectionSet.Selections
                .Where(x => ((Field)x).SelectionSet?.Selections?.Count != 0)
                .ToList();

            if (queryFieldInfo.TypeMetaInfo != null
                && subSubQueries.Count > 0
                && 1 < _options.MaxQueryDepth)
            {
                query = RunSubQueries(query, subSubQueries,
                    queryFieldInfo.TypeMetaInfo, functionTree, 1, resolvedTypes);
            }
            else
            {
                query = functionTree[0].Item1
                        .Invoke(this, new object[] { query, queryFieldInfo.Name }) 
                    as IIncludableQueryable<T, dynamic>;
                query = query!.AsNoTracking();
            }
        }

        return query;
    }

    private readonly MethodInfo _runSubQueriesMethodDefinition = typeof(DynamicQueryLoaderService)
        .GetMethod(nameof(RunSubQueries), BindingFlags.NonPublic | BindingFlags.Instance)!
        .GetGenericMethodDefinition();
    
    private IQueryable<TRootQuery> RunSubQueries<TRootQuery>(IQueryable<TRootQuery> query,
        IList<ISelection> subQueries, 
        TypeMetaInfo queryTypeInfo,
        List<(MethodInfo, string, Type, bool)> functionTree,
        int runningInstance,
        List<Type>? resolvedTypes = null
    )   
        where TRootQuery : class
    {
        var tRootQuery = typeof(TRootQuery);
        
        resolvedTypes ??= new List<Type>();

        for (var index = 0; index < subQueries.Count; index++)
        {
            var subQuery = (Field)subQueries[index];
            
            var subSubQueries = subQuery.SelectionSet!.Selections
                .Where(x => ((Field)x).SelectionSet?.Selections?.Count != 0).ToList();
            
            if (!queryTypeInfo.SubQueries.TryGetValue(subQuery.Name, out var queryFieldInfo)) continue;

            var method = _runThenIncludeGenericMethodDefinition.MakeGenericMethod(tRootQuery,
                queryTypeInfo.BaseType,
                queryFieldInfo.PropertyInfo.PropertyType);
            
            functionTree.Add((method, queryFieldInfo.Name, queryFieldInfo.BaseType, queryFieldInfo.IsEnumerable));
            
            if (queryFieldInfo.TypeMetaInfo != null
                && subSubQueries.Count > 0
                && runningInstance < _options.MaxQueryDepth - 1)
            {
                query = RunSubQueries(query!, subSubQueries,
                    queryFieldInfo.TypeMetaInfo, functionTree,
                    ++runningInstance, resolvedTypes);
            }
            else 
            {
                var instanceToRunTo = runningInstance + 1;
                if (resolvedTypes.All(x => x != queryTypeInfo.BaseType))
                {
                    var reversedFunctionTree = new List<(MethodInfo, string, Type, bool)>(functionTree);
                    reversedFunctionTree.Reverse();
                    var firstNotResolved = reversedFunctionTree
                        .FirstOrDefault(x => resolvedTypes.All(y => y != x.Item3)
                        );
                    if (firstNotResolved != default)
                    {
                        instanceToRunTo = functionTree.IndexOf(firstNotResolved) + 1;
                    }
                }

                while (instanceToRunTo > 2
                       && instanceToRunTo >= functionTree.Count
                       && functionTree[instanceToRunTo - 1].Item3 == functionTree[instanceToRunTo -3].Item3
                       && functionTree[instanceToRunTo - 1].Item4 == functionTree[instanceToRunTo -3].Item4)
                {
                    instanceToRunTo -= 1;
                }

                var lastWasEnumerable = true;
                foreach (var (function, propName, type, isEnumerable) in functionTree.Take(instanceToRunTo))
                {
                    if (function.GetParameters().Length == 2)
                    {
                        query = (function.Invoke(this, new object[] { query, propName }) as
                            IIncludableQueryable<TRootQuery, dynamic>)!;
                    }
                    else
                    {
                        query = (function.Invoke(this, new object[] { query, propName, lastWasEnumerable }) as
                            IIncludableQueryable<TRootQuery, dynamic>)!;
                    }

                    if (resolvedTypes.All(x => x != type))
                    {
                        resolvedTypes.Add(type);
                    }

                    lastWasEnumerable = isEnumerable;
                }
            }
            
            functionTree.RemoveAt(functionTree.Count - 1);
        }

        query.AsNoTrackingWithIdentityResolution();
        return query;
    }

    private readonly MethodInfo _thenIncludeForEnumerableMethodDefinition = typeof(EntityFrameworkQueryableExtensions)
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .First(x => x.Name == "ThenInclude"
                    && x.IsGenericMethod
                    && x.GetParameters()
                        .First()
                        .ParameterType
                        .GetGenericArguments()[1]
                        .IsAssignableTo(typeof(IEnumerable)))
        .GetGenericMethodDefinition();
    
    private readonly MethodInfo _thenIncludeForSingleMethodDefinition = typeof(EntityFrameworkQueryableExtensions)
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .First(x => x.Name == "ThenInclude"
                    && x.IsGenericMethod
                    && !x.GetParameters()
                        .First()
                        .ParameterType
                        .GetGenericArguments()[1]
                        .IsAssignableTo(typeof(IEnumerable)))
        .GetGenericMethodDefinition();
    
    public IIncludableQueryable<TRoot, TProperty> RunThenInclude<TRoot, TEntity, TProperty>
        (IIncludableQueryable<TRoot, dynamic> query, string propertyName, bool isEnumerable)
        where TEntity : class
        where TProperty : class
        where TRoot : class
    {
        if (!_readyToGoExpressions.TryGetValue((typeof(TEntity), propertyName), out var exp))
        {
            exp = BuildPropertyAccessExpression<TEntity, TProperty>(propertyName);
            _readyToGoExpressions.TryAdd((typeof(TEntity), propertyName), exp);
        }

        var methodInfo = isEnumerable
            ? _thenIncludeForEnumerableMethodDefinition
            : _thenIncludeForSingleMethodDefinition;

        methodInfo = methodInfo.MakeGenericMethod(typeof(TRoot), typeof(TEntity), typeof(TProperty));
        
        return (methodInfo.Invoke(null, new object?[] { query, exp }) as
            IIncludableQueryable<TRoot, TProperty>)!;
    }
    
    public IIncludableQueryable<TEntity, TProperty> RunInclude<TEntity, TProperty>(IQueryable<TEntity> query, string propertyName)
        where TEntity : class
        where TProperty : class
    {
        if (!_readyToGoExpressions.TryGetValue((typeof(TEntity), propertyName), out var exp))
        {
            exp = BuildPropertyAccessExpression<TEntity, TProperty>(propertyName);
            _readyToGoExpressions.TryAdd((typeof(TEntity), propertyName), exp);
        }

        return query.Include((exp as Expression<Func<TEntity, TProperty>>)!);
    }

    private Expression<Func<TEntity, TProperty>> BuildPropertyAccessExpression<TEntity, TProperty>(string propertyName)
        where TEntity : class
        where TProperty : class
    {
        var param = Expression.Parameter(typeof(TEntity), "u");
        var expression = Expression.PropertyOrField(param, propertyName); 

        return Expression.Lambda<Func<TEntity, TProperty>>(expression, param);
    }
}