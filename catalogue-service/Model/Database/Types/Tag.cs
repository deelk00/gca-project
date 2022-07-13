namespace CatalogueService.Model.Database.Types;

public class Tag
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public List<Product>? Products { get; set; }
    public List<ProductCategory>? ProductCategories { get; set; }
}