using System.Text.Json;

namespace DynamicQL.Extensions;

public static class JsonElementExtension
{
    public static object? DeserializeJsonElement(this JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Undefined:
            case JsonValueKind.Object:
            {
                var dict = element.Deserialize<Dictionary<string, object?>>();
                return dict?.ToDictionary(x => x.Key, x => x.Value is JsonElement e 
                    ? e.DeserializeJsonElement()
                    : x.Value);
            }
            case JsonValueKind.Array:
                return element.Deserialize<object?[]>()?
                    .Select(x => x is JsonElement e 
                        ? e.DeserializeJsonElement()
                        : x)
                    .ToList();
            case JsonValueKind.String:
                return element.GetString();
            case JsonValueKind.Number:
                {
                    if (element.TryGetInt64(out var l))
                        return l;
                    if (element.TryGetDouble(out var dou))
                        return dou;
                    if (element.TryGetDecimal(out var d))
                        return d;
                }
                break;
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
                return false;
            case JsonValueKind.Null:
                return null;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return null;
    }
}