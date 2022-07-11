using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogueService.Model.Database.Types;

public class ProductCategory
{
<<<<<<< HEAD
    public class ProductCategory
    {
        public Guid Id { get; set; }
        public string Name 
        {
            get => _name; 
            set
            {
                if (value.Length > 128) throw new Exception("String to long");
                _name = value;
            }
        }
        private string _name;


        public List<ProductSubCategory> ProductSubCategories { get; set; }
    }
}
=======
    public Guid Id { get; set; }
    public Guid ParentCategoryId { get; set; }

    public string Name { get; set; }

    [ForeignKey(nameof(ParentCategoryId))]
    public ProductCategory ParentCategory { get; set; }
    
    public List<Tag> Tags { get; set; }
    public List<ProductCategory> ChildCategories { get; set; }
    public List<Product> Products { get; set; }
    public List<FilterPropertyDefinition> FilterPropertyDefinitions { get; set; }
}
>>>>>>> 6099a1ccef0051b1269f63286f0325e7750a87ce
