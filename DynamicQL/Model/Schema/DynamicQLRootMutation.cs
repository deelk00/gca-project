using Microsoft.Extensions.Logging;
using ResolverType = DynamicQL.Attributes.Enums.ResolverType;

namespace DynamicQL.Model.Schema;

public class DynamicQLRootMutation : DynamicQLBaseBuilder
{
    private readonly ILogger<DynamicQLRootMutation> _logger;
    
    public DynamicQLRootMutation(ILogger<DynamicQLRootMutation> logger, IServiceProvider serviceProvider, DynamicQLOptions options)
        : base(logger, serviceProvider, ResolverType.Mutation, options)
    {
        
    }
}