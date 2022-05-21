using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogueService.Model.Database.Types
{
    public class ProductSubCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public List<ProductType> ProductTypes { get; set; }

        [ForeignKey(nameof(ProductCategory))]
        public Guid ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }
    }
}
