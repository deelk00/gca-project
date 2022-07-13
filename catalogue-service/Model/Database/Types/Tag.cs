using System.ComponentModel.DataAnnotations;

namespace CatalogueService.Model.Database.Types;

public class Tag
{
    [Key]
    public Guid Id { get; set; }
    
    [MaxLength(32)]
    public string Name { get; set; }

    public List<Product> Products { get; set; }
    public List<ProductCategory> ProductCategories { get; set; }
}