using DynamicQL.Attributes;
using DynamicQL.Attributes.Enums;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Queries
{
    [DynamicQL(options: QueryOptions.None)]
    public class Login
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public Guid SessionToken { get; set; }

        [DynamicQLQueryResolver("login", false)]
        [DynamicQLResolverArgument("username", typeof(NonNullGraphType<StringGraphType>))]
        public async Task<object> LoginQuery(IResolveFieldContext context, DbContext dbContext)
        {

        }
    }
}
