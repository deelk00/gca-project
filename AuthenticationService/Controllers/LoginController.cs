using System.Security.Claims;
using System.Text;
using AuthenticationService.Model.Database.Types;
using AuthenticationService.Model.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility;
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
        private readonly IConfiguration _config;
        public LoginController(DbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<User>> Get()
        {
            return Ok(new User() { Id = Guid.NewGuid(), Username = "Hallo", Password = "Welt"});
        }

        [HttpPost]
        public async Task<ActionResult<AuthenticationResponse>> Login([FromBody] LoginRequest req)
        {
            var u = await _context.Set<User>().FirstOrDefaultAsync(x => x.Username == req.username);

            if (u == null)
            {
                u = new User()
                {
                    Username = req.username,
                    Password = req.password
                };
                u = await _context.TransactionAsync(x => _context.AddAsync(u));
            }
            
            if (u.Password != req.password)
            {
                return Unauthorized();
            }

            var payload = new List<Claim>()
            {
                new Claim("jti", Guid.NewGuid().ToString()),
                new Claim("sub", u.Id.ToString())
            };

            var (authToken, refreshToken) = Helper.CreateAuthTokenPair(
                Encoding.UTF8.GetBytes(_config.GetValue<string>("Jwt:PrivateKey")),
                _config.GetValue<string>("Jwt:Issuer"),
                _config.GetValue<string>("Jwt:Audience"),
                _config.GetValue<uint>("Jwt:AuthExpiresIn"),
                _config.GetValue<uint>("Jwt:AuthExpiresIn"),
                payload
                );
            
            var res = new AuthenticationResponse
            {
                User = u,
                AuthToken = authToken,
                RefreshToken = refreshToken,
                AuthExpiresIn = _config.GetValue<uint>("Jwt:AuthExpiresIn"),
                RefreshExpiresIn = _config.GetValue<uint>("Jwt:AuthExpiresIn"),
            };

            return res;
        }
        
        [HttpPost]
        public async Task<ActionResult<AuthenticationResponse>> Refresh ([FromBody] string refreshToken)
        {
            
        }
    }
}
