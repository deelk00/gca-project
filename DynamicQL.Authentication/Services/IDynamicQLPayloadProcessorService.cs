using DynamicQL.Authentication.Enums;

namespace DynamicQL.Authentication.Services;

public interface IDynamicQLPayloadProcessorService
{
    Task<Dictionary<PayloadKey, object?>> ProcessPayload(Dictionary<PayloadKey, object?> payload);
    Task<Dictionary<PayloadKey, object?>> GetPayload(object userId);
}