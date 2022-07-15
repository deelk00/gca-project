using System.Reflection;
using DynamicQL.Model.Types;

namespace DynamicQL.Model;

public class DynamicQLOptions
{
    public string Endpoint { get; set; } = "/graphql";
    public List<Assembly> Assemblies { get; set; } = new List<Assembly>();
    public List<Type> DefaultValidationMiddlewareTypes { get; set; } = new List<Type>();
    public List<Type> Types { get; set; } = new List<Type>();
    public DefaultQueryArgumentDefinitions DefaultQueryArguments { get; } = new DefaultQueryArgumentDefinitions();
    public int DefaultPageSize { get; set; } = 50;
    public int MaxQueryDepth { get; set; } = 32;
    public bool ExcludeComplexTypesFromInput { get; set; } = true;
    public int MinimumExecutionTime { get; set; } = 0;
    public bool RunValidationMiddlewaresParallelToQuery { get; set; } = false;
    public bool RunValidationMiddlewaresParallelToEachOther { get; set; } = true;
    public string PrependQuery { get; set; } = "";
    public string PrependCreate { get; set; } = "create";
    public string PrependUpdate { get; set; } = "update";
    public string PrependDelete { get; set; } = "delete";
}