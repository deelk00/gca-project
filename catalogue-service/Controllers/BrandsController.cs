using CatalogueService.Model.Database.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.Api;
using Utility.Api.Controllers;

namespace CatalogueService.Controllers;

[ApiController]
[Route("brands")]
public class BrandsController : CrudController<Brand>
{
    public BrandsController(DbContext context) : base(context)
    {
    }
}