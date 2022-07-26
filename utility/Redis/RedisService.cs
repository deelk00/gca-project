using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;

namespace Utility.Redis;

public class RedisService
{
    private readonly IDistributedCache _distributedCache;
    
    public RedisService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }
    
    public async Task<T?> Get<T>(string key)
    {
        var payload = await _distributedCache.GetAsync(key);
        if (payload == null) return default;
        var serialized = Encoding.UTF8.GetString(payload);
        return JsonSerializer.Deserialize<T>(serialized);
    }

    public async Task Set<T>(string key, T value)
    {
        var serialized = JsonSerializer.Serialize(value);
        await _distributedCache.SetAsync(key, Encoding.UTF8.GetBytes(serialized));
    }
}