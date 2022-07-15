using CatalogueService.Model.Database.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.Api;
using Utility.Api.Controllers;

namespace CatalogueService.Controllers.Relations;

[ApiController]
[Route("relations/tags-and-products")]
public class TagsAndProductsController : ManyToManyRelationShipController<Tag, Product>
{
    public TagsAndProductsController(DbContext context) : base(context, "Products", "Tags")
    {
    }
}