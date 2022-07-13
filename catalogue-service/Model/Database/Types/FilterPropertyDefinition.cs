using System.ComponentModel.DataAnnotations;
using CatalogueService.Model.Database.Enums;

namespace CatalogueService.Model.Database.Types;

public class FilterPropertyDefinition
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; }
    public FilterValueType ValueType { get; set; }

    public List<ProductCategory> ProductCategories { get; set; }
}