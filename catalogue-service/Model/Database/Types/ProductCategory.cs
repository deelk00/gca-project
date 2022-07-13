using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogueService.Model.Database.Types;

public class ProductCategory
{
    [Key]
    public Guid Id { get; set; }
    
    [MaxLength(60)]
    public string Name { get; set; }
    
    [ForeignKey(nameof(ParentCategory))]
    public Guid? ParentCategoryId { get; set; }
    public ProductCategory? ParentCategory { get; set; }

    public List<Tag> Tags { get; set; }
    public List<FilterPropertyDefinition> FilterPropertyDefinitions { get; set; }
    public List<Product> Products { get; set; }
    public List<ProductCategory> ChildrenCategories { get; set; }
}