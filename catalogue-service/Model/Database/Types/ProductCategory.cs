using System.ComponentModel.DataAnnotations.Schema;
using DynamicQL.Attributes;
using DynamicQL.Attributes.Enums;

namespace CatalogueService.Model.Database.Types;

[DynamicQL(options: QueryOptions.SingleCrud | QueryOptions.MultiQuery)]
public class ProductCategory
{
    public Guid Id { get; set; }
    [DynamicQLQueryField(QueryFieldOptions.Equals)]
    public Guid? ParentCategoryId { get; set; }

    public string Name { get; set; }

    [ForeignKey(nameof(ParentCategoryId))]
    public ProductCategory? ParentCategory { get; set; }
    
    public List<Tag>? Tags { get; set; }
    public List<ProductCategory>? ChildCategories { get; set; }
    public List<Product>? Products { get; set; }
    public List<FilterPropertyDefinition>? FilterPropertyDefinitions { get; set; }
}
