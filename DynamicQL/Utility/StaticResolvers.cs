using DynamicQL.Model;
using DynamicQL.Model.Types;
using DynamicQL.Services;
using GraphQL;
using Microsoft.EntityFrameworkCore;

namespace DynamicQL.Utility;

public static class StaticResolvers
{
    public static async Task<object?> GeneralSingleCreateMutationResolver<T>(IResolveFieldContext fieldContext, 
        DbContext dbContext,  
        IDynamicQueryLoaderService queryLoaderService,
        TypeMetaInfo info)
        where T : class
    {
        var entityArgument = fieldContext.Arguments?.FirstOrDefault();
        
        if (entityArgument?.Value.Value is not T entity) throw new ExecutionError($"Could not parse input to {typeof(T).Name}");

        entity = (await dbContext.Set<T>().AddAsync(entity)).Entity;

        await dbContext.SaveChangesAsync();
        
        return entity;
    }
    
    public static async Task<object?> GeneralMultiCreateMutationResolver<T>(IResolveFieldContext fieldContext, 
        DbContext dbContext,  
        IDynamicQueryLoaderService queryLoaderService,
        TypeMetaInfo info)
    {
        throw new Exception();
    }
    
    public static async Task<object?> GeneralSingleUpdateMutationResolver<T>(IResolveFieldContext fieldContext, 
        DbContext dbContext,  
        IDynamicQueryLoaderService queryLoaderService,
        TypeMetaInfo info)
    {
        throw new Exception();
    }
    
    public static async Task<object?> GeneralMultiUpdateMutationResolver<T>(IResolveFieldContext fieldContext, 
        DbContext dbContext,  
        IDynamicQueryLoaderService queryLoaderService,
        TypeMetaInfo info)
    {
        throw new Exception();
    }
    
    public static async Task<object?> GeneralSingleDeleteByIdMutationResolver<T>(IResolveFieldContext fieldContext, 
        DbContext dbContext,  
        IDynamicQueryLoaderService queryLoaderService,
        TypeMetaInfo info)
    {
        throw new Exception();
    }
    
    public static async Task<object?> GeneralMultiDeleteByIdMutationResolver<T>(IResolveFieldContext fieldContext, 
        DbContext dbContext,  
        IDynamicQueryLoaderService queryLoaderService,
        TypeMetaInfo info)
    {
        throw new Exception();
    }
    
    public static async Task<object?> GeneralSingleDeleteByEntityMutationResolver<T>(IResolveFieldContext fieldContext, 
        DbContext dbContext,  
        IDynamicQueryLoaderService queryLoaderService,
        TypeMetaInfo info)
    {
        throw new Exception();
    }
    
    public static async Task<object?> GeneralMultiDeleteByEntityMutationResolver<T>(IResolveFieldContext fieldContext, 
        DbContext dbContext,  
        IDynamicQueryLoaderService queryLoaderService,
        TypeMetaInfo info)
    {
        throw new Exception();
    }
    
    public static async Task<object?> GeneralSingleQueryResolver<T>(IResolveFieldContext fieldContext, DbContext dbContext,  
        IDynamicQueryLoaderService queryLoaderService, DynamicQLOptions options, TypeMetaInfo info)
        where T : class
    {
        var id = fieldContext.GetArgument<object>(options.DefaultQueryArguments.Id.Name);

        if (id == null)
        {
            throw new ArgumentException("id was null");
        }

        var resolver = info.BuildIdEqualsExpression<T>(id);
        if (resolver == null)
            return null;
        
        return await queryLoaderService
            .LoadDynamicQueries<T>(dbContext, fieldContext)
            .FirstOrDefaultAsync(resolver);
    }
    
    public static async Task<object?> GeneralMultiQueryResolver<T>(IResolveFieldContext fieldContext, DbContext dbContext, 
        IDynamicQueryLoaderService queryLoaderService, DynamicQLOptions options, TypeMetaInfo info)
        where T : class
    {
        var pageSize = fieldContext.GetArgument(options.DefaultQueryArguments.PageSize.Name, 
            info.PageSize ?? options.DefaultPageSize);

        var query = queryLoaderService.LoadDynamicQueries<T>(dbContext, fieldContext, true);
        
        var page = fieldContext.GetArgument(options.DefaultQueryArguments.Page.Name, 0);
        var skip = fieldContext.GetArgument(options.DefaultQueryArguments.Skip.Name, 0);
        var take = fieldContext.GetArgument(options.DefaultQueryArguments.Take.Name, 0);

        if (fieldContext.Arguments != null)
            foreach (var (name, value) in fieldContext.Arguments)
            {
                if (value.Value == null
                    || name == options.DefaultQueryArguments.Page.Name
                    || name == options.DefaultQueryArguments.Skip.Name
                    || name == options.DefaultQueryArguments.Take.Name) continue;

                if (info.QueryFields.TryGetValue(name, out var queryField))
                {
                    query = query.Where(queryField.BuildExpression<T>(value.Value));
                }
            }

        IOrderedQueryable<T> orderedQuery = null;
        
        for (var i = 0; i < info.OrderByFields.Count; i++)
        {
            var field = info.OrderByFields[i] as dynamic;
            if (i == 0)
            {
                orderedQuery = field.RunOrderBy(query);
            }
            else
            {
                orderedQuery = field.RunThenBy(orderedQuery);
            }
        }

        if(orderedQuery != null)
            query = orderedQuery;
        switch (skip)
        {
            case < 1 when take < 1:
                query = query.Skip(pageSize * page)
                    .Take(pageSize);
                break;
            case < 1:
                query = query.Skip(pageSize * page)
                    .Take(take);
                break;
            default:
            {
                if (take < 1)
                {
                    query = query.Skip(skip)
                        .Take(pageSize);
                }
                else
                {
                    query = query.Skip(skip)
                        .Take(take);
                }

                break;
            }
        }
        
        return await query.ToListAsync();
    }
}