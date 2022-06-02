using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogueService.Model.Database.Types
{
    public class ProductType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }


        [ForeignKey(nameof(ProductCategory))]
        public Guid ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }

        public List<Product> Products { get; set; }
    }
}
