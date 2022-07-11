using CatalogueService.Model.Database.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.Api;

namespace CatalogueService.Controllers;

[ApiController]
[Route("filter-properties")]
public class FilterPropertiesController : CrudController<FilterProperty>
{
    public FilterPropertiesController(DbContext context) : base(context)
    {
    }
}