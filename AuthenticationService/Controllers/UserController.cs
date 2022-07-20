using AuthenticationService.Model.Database.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.Api.Controllers;

namespace AuthenticationService.Controllers;

[ApiController]
[Route("users")]
public class UserController : CrudController<User>
{
    public UserController(DbContext context) : base(context)
    {
    }

    [HttpPut]
    public override async Task<ActionResult<User>> Post(User entity)
    {
        if (!string.IsNullOrEmpty(entity.Password)) return await base.Post(entity);
        
        var e = await Context.FindAsync<User>(entity.Id);
        if (e == null) return BadRequest();
        entity.Password = e.Password;
        return await base.Post(entity);
    }
}