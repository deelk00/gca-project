using System.ComponentModel.DataAnnotations.Schema;
using DynamicQL.Attributes;
using DynamicQL.Attributes.Enums;

namespace CatalogueService.Model.Database.Types;

[DynamicQL(options: QueryOptions.SingleCrud | QueryOptions.MultiQuery)]
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