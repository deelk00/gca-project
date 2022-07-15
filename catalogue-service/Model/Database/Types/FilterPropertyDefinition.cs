using CatalogueService.Model.Database.Enums;
using DynamicQL.Attributes;
using DynamicQL.Attributes.Enums;

namespace CatalogueService.Model.Database.Types;

[DynamicQL(options: QueryOptions.SingleCrud | QueryOptions.MultiQuery)]
public class FilterPropertyDefinition
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public FilterPropertyValueType ValueType { get; set; }

    public List<FilterProperty>? FilterProperties { get; set; }
    public List<ProductCategory>? ProductCategories { get; set; }
}