namespace CatalogueService.Model.Database.Types;

public class FilterProperty
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid FilterPropertyDefinitionId { get; set; }
    public string Value { get; set; }

    public Product Product { get; set; }
    public FilterPropertyDefinition FilterPropertyDefinition { get; set; }
}