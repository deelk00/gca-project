using CatalogueService.Model.Database.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.Api;
using Utility.Api.Controllers;

namespace CatalogueService.Controllers;

[ApiController]
[Route("products")]
[Authorize]
public class ProductsController : CrudController<Product>
{
    public ProductsController(DbContext context) : base(context)
    {
    }
}