using KnowledgeMining.Data;
using KnowledgeMining.Helpers;
using KnowledgeMining.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace KnowledgeMining.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UsersController : ControllerBase
    {
        private readonly JwtTokenGenerator _generateJwtToken;
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository, JwtTokenGenerator generateJwtToken)
        {
            _userRepository = userRepository;
            _generateJwtToken = generateJwtToken;
        }




        [HttpPost("login")]
        [AllowAnonymous] 
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var user = await _userRepository.GetUserByUsernameAndPasswordAsync(loginModel.Username, loginModel.Password);

            if (user.Id == 0 || user.Id == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            var roles = await _userRepository.GetUserRolesAsync(user.RoleId);

         
            string rolesString = roles != null ? roles.ToString() : string.Empty;


            var token = _generateJwtToken.GenerateToken(user, rolesString);

            return Ok(new
            {
                Token = token,
                Role= roles,
                User= user
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            var userId = await _userRepository.CreateUserAsync(user);
            user.Id = userId;
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(User user)
        {
            var updatedUser = await _userRepository.UpdateUserAsync(user);

            if (updatedUser == null)
            {
                return NotFound();
            }

            return Ok(updatedUser);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result =   await _userRepository.DeleteUserAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
