namespace CatalogueService.Model.Database.Types;

public class Image
{
    public Guid Id { get; set; }
    public byte[] Data { get; set; }
    public string Hash { get; set; }

    public List<Brand> Brands { get; set; }
}