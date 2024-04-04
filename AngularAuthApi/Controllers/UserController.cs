using AngularAuthApi.Context;
using AngularAuthApi.Helper;
using AngularAuthApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.RegularExpressions;

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

            //Check Email 

            if (await CheckEmailExistAsync(userObj.Email))
                return BadRequest(new { Message = "Email already exist" });

            //Check Password
            var pass = CheckPasswordStrength(userObj.Password);
            if (!string.IsNullOrEmpty(pass))
                return BadRequest(new { Message = pass.ToString()});

            userObj.Password = PasswordHasher.HashPassword(userObj.Password);
            userObj.Role = "User";
            userObj.Token = "";
            await _authDbContext.Users.AddAsync(userObj);
            await _authDbContext.SaveChangesAsync();
            return Ok(new { Message = "User Registered!" });

        }

        private Task<bool> CheckEmailExistAsync(string email) => _authDbContext.Users.AnyAsync(x => x.Email == email);

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb =new StringBuilder();
            if(password.Length < 8)
                sb.Append("Minimum password length should ne 8"+Environment.NewLine);
            if ((Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
                sb.Append("Password should be Alphanumeric"+ Environment.NewLine);
            if (!Regex.IsMatch(password, "[<,>,@,!,#,$,%,^,&,*,(,),_,-,\\[,\\],{,},?,;,:,',~,`,=]"))
                sb.Append("Password should contain special char" + Environment.NewLine);

            return sb.ToString();
        }
    }
}
