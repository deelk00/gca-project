namespace CatalogueService.Model.Database.Types
{
    public class ProductCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public List<ProductSubCategory> ProductSubCategories { get; set; }
    }
}
