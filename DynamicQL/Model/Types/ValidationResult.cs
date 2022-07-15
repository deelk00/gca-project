namespace DynamicQL.Model.Types;

public class ValidationResult
{
    public bool IsValidated { get; set; } = false;
    public string? ErrorMessage { get; set; } = "";
}