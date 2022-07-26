using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Utility.Redis;

namespace Utility.Extensions;

public static class ServiceCollectionExtension
{
    public static void AddRedis(this WebApplicationBuilder builder)
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetValue<string>("Redis:Url");
            options.InstanceName = builder.Configuration.GetValue<string>("Redis:AppName");
        });
        builder.Services.AddSingleton<RedisService>();
    }
}