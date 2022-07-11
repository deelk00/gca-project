using CatalogueService.Model.Database.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.Api;

namespace CatalogueService.Controllers.Relations;

[ApiController]
[Route("relations/tags-and-products")]
public class BrandsProductsController : ManyToManyRelationShipController<Tag, Product>
{
    public BrandsProductsController(DbContext context) : base(context, "Products", "Tags")
    {
    }
}