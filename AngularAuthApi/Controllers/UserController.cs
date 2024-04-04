using AngularAuthApi.Context;
using AngularAuthApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AngularAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDBContext _authDbContext;
        public UserController(AppDBContext authDbContext)
        {
            this._authDbContext = authDbContext;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();

            var user = await _authDbContext.Users.FirstOrDefaultAsync(x=>x.Email == userObj.Email && x.Password == userObj.Password);
            if (user == null)
                return NotFound(new { Message = "User Not Found" });

            return Ok(new { Message = "Login Success!" });
        }

        [HttpPost("register")]

        public async Task<IActionResult> RegisterUser([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();

            await _authDbContext.Users.AddAsync(userObj);
            await _authDbContext.SaveChangesAsync();
            return Ok(new { Message = "User Registered!" });

        }
    }
}
