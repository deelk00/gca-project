using CatalogueService.Model.Database.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.Api.Controllers;

namespace CatalogueService.Controllers;

[ApiController]
[Route("product-image")]
public class ProductImageController : CrudController<ProductImage>
{
    public ProductImageController(DbContext context) : base(context)
    {
    }
}