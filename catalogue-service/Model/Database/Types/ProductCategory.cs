namespace CatalogueService.Model.Database.Types
{
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
