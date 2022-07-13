using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogueService.Model.Database.Types;

public class Image
{
    public Guid Id { get; set; }
    public string Hash { get; set; }
    [ForeignKey(nameof(DatabaseImage))]
    public Guid DatabaseImageId { get; set; }
    
    public DatabaseImage? DatabaseImage { get; set; }
    public List<Brand>? Brands { get; set; }
}