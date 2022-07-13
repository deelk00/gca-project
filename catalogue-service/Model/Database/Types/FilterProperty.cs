using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogueService.Model.Database.Types;

public class FilterProperty
{
    public Guid Id { get; set; }
    [ForeignKey(nameof(Product))]
    public Guid ProductId { get; set; }
    [ForeignKey(nameof(FilterPropertyDefinition))]
    public Guid FilterPropertyDefinitionId { get; set; }
    public string Value { get; set; }

    public Product? Product { get; set; }
    public FilterPropertyDefinition? FilterPropertyDefinition { get; set; }
}