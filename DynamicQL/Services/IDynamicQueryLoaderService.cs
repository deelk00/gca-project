using GraphQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace DynamicQL.Services;

public interface IDynamicQueryLoaderService
{
    public IQueryable<T> LoadDynamicQueries<T>(DbContext dbContext, IResolveFieldContext fieldContext, bool? isMultiple = null)
        where T : class;
}