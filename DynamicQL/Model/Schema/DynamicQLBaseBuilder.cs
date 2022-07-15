using System.Reflection;
using DynamicQL.Attributes;
using DynamicQL.Attributes.Enums;
using DynamicQL.Extensions;
using DynamicQL.Model.Types;
using DynamicQL.Services;
using DynamicQL.Utility;
using GraphQL;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ResolverType = DynamicQL.Attributes.Enums.ResolverType;

namespace DynamicQL.Model.Schema;

public class DynamicQLBaseBuilder : ObjectGraphType
{
    protected readonly ILogger Logger;
    protected readonly IServiceProvider ServiceProvider;
    protected readonly ResolverType ResolverType;
    protected readonly DynamicQLOptions Options;

    private readonly MethodInfo _queryResolverBuilderMethodInfo = typeof(DynamicQLBaseBuilder)
        .GetMethod(nameof(QueryResolverBuilder))!
        .GetGenericMethodDefinition();
    
    public DynamicQLBaseBuilder(ILogger logger, 
        IServiceProvider serviceProvider, 
        ResolverType resolverType, 
        DynamicQLOptions options)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;
        ResolverType = resolverType;
        Options = options;

        var fieldInfos = StaticData.TypeToGraphQLTypeMetaInfoMap;
        foreach (var info in fieldInfos.Values)
        {
            var resolvers = resolverType.HasFlag(ResolverType.Mutation)
                ? info.MutationResolvers
                : resolverType.HasFlag(ResolverType.Query)
                    ? info.QueryResolvers
                    : null;

            if (resolvers == null)
                throw new Exception("DynamicQL has run into an errrrrrrrrrr");

            var setupMethods = info.BaseType
                .GetMethods()
                .Where(x => x.GetCustomAttribute<DynamicQLQuerySetupAttribute>() != null)
                .OrderBy(x => x.GetCustomAttribute<DynamicQLQuerySetupAttribute>()!.ExecutionOrder);

            var resolversToSkip = new List<string?>();
            
            foreach (var setupMethod in setupMethods)
            {
                var parameters = setupMethod?
                    .GetParameters()
                    .Select(param =>
                        param.ParameterType == typeof(TypeMetaInfo)
                            ? info
                            : serviceProvider.GetRequiredService(param.ParameterType));

                var setupResult = setupMethod?.Invoke(Activator.CreateInstance(info.BaseType), parameters?.ToArray());
                
                if(setupResult is not SetupResult result)
                {
                    throw new ArgumentException(
                        $"Return type of setup function: {setupMethod?.Name} " +
                        $"on type: {info.BaseType} has to be type of {typeof(SetupResult).FullName}"
                        );
                }
                
                switch (result)
                {
                    case SetupResult.NotSuccessful:
                        logger.LogError($"Error in setup function: {setupMethod?.Name} on type: {info.BaseType}");
                        break;
                    
                    case SetupResult.DoNotAddQuery:
                        resolversToSkip.Add(setupMethod?.GetCustomAttribute<DynamicQLQuerySetupAttribute>()?.ResolverName);
                        break;
                    
                    case SetupResult.Throw:
                        throw new Exception($"Error in setup function: {setupMethod?.Name} on type: {info.BaseType}");
                        
                    default:
                    case SetupResult.Successful:
                    case SetupResult.AddQuery:
                        break;
                }
            }

            foreach (var resolverDefinition in resolvers)
            {
                if (resolversToSkip.Contains(resolverDefinition.Resolver.Name))
                {
                    continue;
                }
                
                var resolver = _queryResolverBuilderMethodInfo
                    .MakeGenericMethod(info.BaseType)
                    .Invoke(this, new object[]
                    {
                        resolverDefinition.Resolver,
                        Activator.CreateInstance(info.BaseType)!, 
                        info
                    }) as Func<IResolveFieldContext, object?>;

                Field(resolverDefinition.IsEnumerableReturnType ? info.MultiQueryGraphType : info.SingleQueryGraphType,
                    resolverDefinition.Name,
                    resolverDefinition.Description,
                    resolverDefinition.QueryArguments,
                    resolver,
                    resolverDefinition.DeprecatedReason);
            }
        }
        
        logger.LogInformation("The query has been successfully initialized");
    }

    private List<Type> GetValidationMiddlewares()
    {
        var types = Options.DefaultValidationMiddlewareTypes;
        types.AddRange(Options.Types
            .Concat(Options.Assemblies.SelectMany(x => x.GetTypes())));

        types = types
            .Where(y => y.IsAssignableTo(typeof(IDynamicQLValidationMiddleware)))
            .ToList();
        
        return types;
    }

    public readonly MethodInfo _getFromServiceProviderMethodInfo = typeof(DynamicQLRootQuery)
        .GetMethod(nameof(GetFromServiceProvider))!
        .GetGenericMethodDefinition();
    public readonly MethodInfo _getRequiredFromServiceProviderMethodInfo = typeof(DynamicQLRootQuery)
        .GetMethod(nameof(GetRequiredFromServiceProvider))!
        .GetGenericMethodDefinition();
    
    public object? GetFromServiceProvider<T>(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetService<T>();
    }
    
    public object GetRequiredFromServiceProvider<T>(IServiceProvider serviceProvider) where T : notnull
    {
        return serviceProvider.GetRequiredService<T>();
    }
    
    public Func<IResolveFieldContext, object?> QueryResolverBuilder<T>(MethodInfo methodInfo, object target, TypeMetaInfo info)
    {
        var parameters = methodInfo.GetParameters().Skip(1); // skip IResolveFieldContext

        var funcList = new List<Func<IServiceProvider, object>>();
        foreach (var parameterInfo in parameters)
        {
            if (parameterInfo.ParameterType == typeof(TypeMetaInfo))
            {
                funcList.Add((x) => info);
            }
            else if (parameterInfo.IsNullable())
            {
                funcList.Add(_getFromServiceProviderMethodInfo.MakeGenericMethod(parameterInfo.ParameterType)
                    .CreateDelegate<Func<IServiceProvider, object>>(this));
            }
            else
            {
                funcList.Add(_getRequiredFromServiceProviderMethodInfo.MakeGenericMethod(parameterInfo.ParameterType)
                    .CreateDelegate<Func<IServiceProvider, object>>(this));
            }
        }

        return ResolveWrapper;
            
        async Task<object?> ResolveWrapper (IResolveFieldContext context)
        {
            Task? minExecTimeTask = null;

            var minExecTime = Math.Max(Options.MinimumExecutionTime, info.MinimumExecutionTime);
            if (Options.MinimumExecutionTime > 0 || info.MinimumExecutionTime > 0)
            {
                minExecTimeTask = Task.Delay(minExecTime);
            }

            var serviceProvider = ServiceProvider.CreateScope().ServiceProvider;
            var paras = funcList.Select(x => x(serviceProvider)).Prepend(context).ToArray();

            IDbContextTransaction transaction = null;

            var validationMiddlewareTasks = new List<Task<ValidationResult>>();

            if (info.ExcludedValidationMiddlewares.All(x => x != null))
            {
                foreach (var validationMiddlewareType in Options.DefaultValidationMiddlewareTypes.Concat(info.ValidationMiddlewares))
                {
                    if(info.ExcludedValidationMiddlewares.Contains(validationMiddlewareType))
                        continue;

                    var validator =
                        serviceProvider.GetRequiredService(validationMiddlewareType) as IDynamicQLValidationMiddleware;

                    var validationTask = validator!.InvokeAsync<T>(context);
                
                    validationMiddlewareTasks.Add(validationTask);
                    if (Options.RunValidationMiddlewaresParallelToEachOther)
                        await validationTask;
                }
            }
            

            // check validation before executing
            if (!Options.RunValidationMiddlewaresParallelToQuery)
            {
                await Task.WhenAll(validationMiddlewareTasks);
                if (validationMiddlewareTasks.FirstOrDefault(isAuthenticated => 
                        !isAuthenticated.Result.IsValidated)?.Result is { } beforeValidationResult)
                {
                    throw new ExecutionError(beforeValidationResult.ErrorMessage);
                }
            }
            
            if (paras.FirstOrDefault(x => x is DbContext) is DbContext dbContext)
            {
                if (ResolverType.HasFlag(ResolverType.Query))
                {
                    dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTrackingWithIdentityResolution;
                }
                else if (ResolverType.HasFlag(ResolverType.Mutation))
                {
                    transaction = await dbContext.Database.BeginTransactionAsync();
                }
            }

            object? result = null;
            try
            {
                if (methodInfo.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null)
                {
                    result = await (methodInfo.Invoke(target, paras) as Task<object?>)!;
                }
                else
                {
                    result = methodInfo.Invoke(target, paras)!;
                }
                
                // check validation after execution
                if (Options.RunValidationMiddlewaresParallelToQuery)
                {
                    await Task.WhenAll(validationMiddlewareTasks);
                    if (validationMiddlewareTasks.FirstOrDefault(isAuthenticated => 
                            !isAuthenticated.Result.IsValidated)?.Result is { } afterValidationResult)
                    {
                        throw new ExecutionError(afterValidationResult.ErrorMessage);
                    }
                }
                
                if(transaction != null)
                    await transaction.CommitAsync();
            }
            catch(Exception ex)
            {
                if(transaction != null)
                    await transaction.RollbackAsync();
                throw;
            }

            if (minExecTimeTask != null)
                await minExecTimeTask;

            return result;
        };
    }
}