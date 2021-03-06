using CatalogueService.Model.Database.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.Api;
using Utility.Api.Controllers;

namespace CatalogueService.Controllers;

[ApiController]
[Route("currencies")]
public class CurrenciesController : CrudController<Currency>
{
    public CurrenciesController(DbContext context) : base(context)
    {
    }
}