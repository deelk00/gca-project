using System.ComponentModel.DataAnnotations.Schema;
using CatalogueService.Model.Database.Enums;

namespace CatalogueService.Model.Database.Types;

public class Product
{
    public Guid Id { get; set; }
    [ForeignKey(nameof(ProductCategory))]
    public Guid ProductCategoryId { get; set; }
    
    public string Name { get; set; }
    public uint Stock { get; set; }
    public Gender Gender { get; set; }
    public decimal Price { get; set; }

    public ProductCategory ProductCategory { get; set; }
    public List<Tag> Tags { get; set; }
    public List<FilterProperty> FilterProperties { get; set; }
}