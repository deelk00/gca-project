using AuthenticationService.Model.Database.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.EFCore;
using Utility.Other.Extensions;

namespace AuthenticationService.Controllers
{
    public record LoginRequest(string username, string password);
    
    [ApiController]
    [Route("login")]
    public class LoginController : Controller
    {
        private readonly DbContext _context;
        public LoginController(DbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<User>> Get()
        {
            return Ok(new User() { Id = Guid.NewGuid(), Username = "Hallo", Password = "Welt"});
        }

        [HttpPost]
        public async Task<ActionResult<User>> Login([FromBody] LoginRequest req)
        {
            var u = await _context.Set<User>().FirstOrDefaultAsync(x => x.Username == req.username);
            if (u != null)
            {
                if(u.Password == req.password) return Ok(u.ToResponseDict());
                return Unauthorized();
            }
            
            u = new User()
            {
                Username = req.username,
                Password = req.password
            };
            u = await _context.TransactionAsync(x => _context.AddAsync(u));
            u.Password = "";

            return u;
        }
    }
}
