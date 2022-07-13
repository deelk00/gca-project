using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CatalogueService.Model.Database.Enums;

namespace CatalogueService.Model.Database.Types;

public class Product
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(120)]
    public string Name { get; set; }
    
    public int Stock { get; set; }
    public decimal Price { get; set; }
    public Gender Gender { get; set; }

    [ForeignKey(nameof(ProductCategory))]
    public Guid ProductCategoryId { get; set; }
    public ProductCategory ProductCategory { get; set; }

    public List<FilterProperty> FilterProperties { get; set; }
    public List<Tag> Tags { get; set; }
}