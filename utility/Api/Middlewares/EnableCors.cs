using Microsoft.AspNetCore.Http;

namespace Utility.Api.Middlewares;

public class EnableCors : IMiddleware
{
    public EnableCors()
    {
        
    }
    
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Request.Headers.Add("Access-Control-Allow-Origin", "*");
        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        return next(context);
    }
}