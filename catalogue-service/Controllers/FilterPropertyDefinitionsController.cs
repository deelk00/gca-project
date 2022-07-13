using CatalogueService.Model.Database.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.Api;

namespace CatalogueService.Controllers;

[ApiController]
[Route("filter-property-definitions")]
public class FilterPropertyDefinitionsController : CrudController<FilterPropertyDefinition>
{
    public FilterPropertyDefinitionsController(DbContext context) : base(context)
    {
    }
}