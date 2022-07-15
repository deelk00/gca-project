using DynamicQL.Model.Types;
using GraphQL;

namespace DynamicQL.Services;

public interface IDynamicQLValidationMiddleware
{
    /// <summary>
    /// Method which is invoked before the document is executed and returns if the document should be executed
    /// </summary>
    /// <param name="fieldContext">Field context of the request</param>
    /// <typeparam name="T">Type of the root query</typeparam>
    /// <returns>true if validated, false if not</returns>
    Task<ValidationResult> InvokeAsync<T>(IResolveFieldContext fieldContext);
}