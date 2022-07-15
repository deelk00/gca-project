using CatalogueService.Model.Database.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.Api;
using Utility.Api.Controllers;

namespace CatalogueService.Controllers;

[ApiController]
[Route("tags")]
public class TagsController : CrudController<Tag>
{
    public TagsController(DbContext context) : base(context)
    {
    }
}