using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogueService.Model.Database.Types;

public class Brand
{
    public Guid Id { get; set; }
    [ForeignKey(nameof(Image))]
    public Guid ImageId { get; set; }
    public string Name { get; set; }

    public Image? Image { get; set; }
}