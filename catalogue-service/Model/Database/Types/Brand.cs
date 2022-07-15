using System.ComponentModel.DataAnnotations.Schema;
using DynamicQL.Attributes;
using DynamicQL.Attributes.Enums;

namespace CatalogueService.Model.Database.Types;

[DynamicQL(options: QueryOptions.SingleCrud | QueryOptions.MultiQuery)]
public class Brand
{
    public Guid Id { get; set; }
    [ForeignKey(nameof(Image))]
    public Guid ImageId { get; set; }
    public string Name { get; set; }

    public Image? Image { get; set; }

    public List<Product>? Products { get; set; }
}