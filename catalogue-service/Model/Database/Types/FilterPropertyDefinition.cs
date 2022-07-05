using CatalogueService.Model.Database.Enums;

namespace CatalogueService.Model.Database.Types;

public class FilterPropertyDefinition
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public FilterPropertyValueType ValueType { get; set; }

    public List<FilterProperty> FilterProperties { get; set; }
    public List<ProductCategory> ProductCategories { get; set; }
}