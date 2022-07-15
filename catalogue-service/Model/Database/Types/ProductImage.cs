using System.ComponentModel.DataAnnotations.Schema;
using DynamicQL.Attributes;
using DynamicQL.Attributes.Enums;

namespace CatalogueService.Model.Database.Types;

[DynamicQL(options: QueryOptions.SingleCrud | QueryOptions.MultiQuery)]
public class ProductImage
{
    public Guid Id { get; set; }
    
    [ForeignKey(nameof(Image))]
    public Guid ImageId { get; set; }
    [ForeignKey(nameof(Product))]
    public Guid ProductId { get; set; }
    
    public byte index { get; set; }
    
    public Image? Image { get; set; }
    public Product? Product { get; set; }
}