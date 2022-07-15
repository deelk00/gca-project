using Microsoft.Extensions.Logging;
using ResolverType = DynamicQL.Attributes.Enums.ResolverType;

namespace DynamicQL.Model.Schema;

public class DynamicQLRootQuery : DynamicQLBaseBuilder
{
    public DynamicQLRootQuery(ILogger<DynamicQLRootQuery> logger, IServiceProvider serviceProvider, DynamicQLOptions options)
        : base(logger, serviceProvider, ResolverType.Query, options)
    {
    }

    
}