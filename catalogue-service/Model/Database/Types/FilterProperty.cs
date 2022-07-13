using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogueService.Model.Database.Types;

public class FilterProperty
{
    [Key]
    public Guid Id { get; set; }
    
    public string Value { get; set; }

    [ForeignKey(nameof(Product))]
    public Guid ProductId { get; set; }
    public Product Product { get; set; }

    [ForeignKey(nameof(FilterPropertyDefinition))]
    public Guid FilterPropertyDefinitionId { get; set; }
    public FilterPropertyDefinition FilterPropertyDefinition { get; set; }
}