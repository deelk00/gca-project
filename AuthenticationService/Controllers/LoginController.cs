﻿using AuthenticationService.Model.Database.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.EFCore;

namespace AuthenticationService.Controllers
{
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
        public async Task<ActionResult<User>> Login([FromBody] User user)
        {
            var u = await _context.Set<User>().FirstOrDefaultAsync(x => x.Username == user.Username) 
                    ?? await _context.TransactionAsync(x => _context.AddAsync(user));

            return u;
        }
    }
}