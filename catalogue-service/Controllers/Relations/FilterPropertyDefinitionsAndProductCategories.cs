using CatalogueService.Model.Database.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.Api;

namespace CatalogueService.Controllers.Relations
{
    [ApiController]
    [Route("relations/filter-property-definitions-and-product-categories")]
    public class FilterPropertyDefinitionsAndProductCategories : ManyToManyRelationShipController<FilterPropertyDefinition, ProductCategory>
    {
        public FilterPropertyDefinitionsAndProductCategories(DbContext context) : base(context, "ProductCategories", "FilterPropertyDefinitions")
        {
        }
    }
}
