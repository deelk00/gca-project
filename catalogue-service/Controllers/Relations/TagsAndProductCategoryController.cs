using CatalogueService.Model.Database.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.Api;

namespace CatalogueService.Controllers.Relations
{
    [ApiController]
    [Route("relations/tags-and-product-categories")]
    public class TagsAndProductCategoryController : ManyToManyRelationShipController<Tag, ProductCategory>
    {
        public TagsAndProductCategoryController(DbContext context) : base(context, "ProductCategories", "Tags")
        {
        }
    }
}
