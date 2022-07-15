using DynamicQL.Model;
using DynamicQL.Model.Requests;
using DynamicQL.Model.Schema;
using GraphQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicQL.Extensions;

public static class WebApplicationExtension
{
    public static WebApplication UseDynamicGraphQL(this WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<DynamicQLOptions>();
        
        
        app.MapPost(options.Endpoint, HandleGraphQLRequest);
        
        return app;
    }

    private static async Task HandleGraphQLRequest(
        DynamicQLSchema schema,
        HttpContext httpContext,
        GraphQLQueryRequest request
        )
    {
        var userContext = new Dictionary<string, object?>();
        userContext.Add("HttpContext", httpContext);

        var result = await new DocumentExecuter().ExecuteAsync(_ =>
        {
            _.Schema = schema;
            _.Query = request.Query;
            _.Inputs = request.Variables != null ? new Inputs(
                request.Variables.ToDictionary(x => x.Key, 
                    x => x.Value.DeserializeJsonElement())
                ) : null;
            _.UserContext = userContext;
        });

        var writer = new GraphQL.SystemTextJson.DocumentWriter();
        await writer.WriteAsync(httpContext.Response.Body, result);
        
        if (result.Errors?.Count > 0)
        {
            Console.WriteLine(result.Errors.First().InnerException);
            Console.WriteLine(result.Errors.First().StackTrace);
        }
    }
}