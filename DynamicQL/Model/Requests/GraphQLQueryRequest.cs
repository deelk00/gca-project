using System.Text.Json;
using GraphQL;

namespace DynamicQL.Model.Requests;

public class GraphQLQueryRequest
{
    public string? OperationName { get; set; }
    public string? NamedQuery { get; set; }
    public string Query { get; set; }
    public Dictionary<string, JsonElement>? Variables { get; set; }
}