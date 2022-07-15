using DynamicQL.Attributes;
using DynamicQL.Attributes.Enums;

namespace CatalogueService.Model.Database.Types;

[DynamicQL(options: QueryOptions.SingleCrud | QueryOptions.MultiQuery)]
public class Tag
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public List<Product>? Products { get; set; }
    public List<ProductCategory>? ProductCategories { get; set; }
}