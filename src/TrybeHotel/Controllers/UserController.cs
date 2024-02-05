using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using TrybeHotel.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("user")]

    public class UserController : Controller
    {
        private readonly IUserRepository _repository;
        public UserController(IUserRepository repository)
        {
            _repository = repository;
        }
        
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = "Admin")]
        public IActionResult GetUsers()
        {
            var token = HttpContext.User.Identity as ClaimsIdentity;
            var role = token?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            if (role != "admin") return Unauthorized();

            var users = _repository.GetUsers();
            return Ok(users);
        }

        [HttpPost]
        public IActionResult Add([FromBody] UserDtoInsert user)
        {
            var userExists = _repository.GetUserByEmail(user.Email!);
            if (userExists != null)
            {
                return Conflict(new { message = "User email already exists" });
            }
            var newUser = _repository.Add(user);
            return Created("", newUser);
        }
    }
}