using System.ComponentModel.DataAnnotations.Schema;
using DynamicQL.Attributes;
using DynamicQL.Attributes.Enums;

namespace CatalogueService.Model.Database.Types;

[DynamicQL(options: QueryOptions.SingleCrud | QueryOptions.MultiQuery)]
public class Image
{
    public Guid Id { get; set; }
    public string Hash { get; set; }
    [ForeignKey(nameof(DatabaseImage))]
    public Guid DatabaseImageId { get; set; }
    
    [DynamicQLExclude]
    public DatabaseImage? DatabaseImage { get; set; }
    public List<Brand>? Brands { get; set; }
    public List<ProductImage>? ProductImages { get; set; }
}