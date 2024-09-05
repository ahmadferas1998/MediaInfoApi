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
    public class RoleController : ControllerBase
    {
        private readonly JwtTokenGenerator _generateJwtToken;
        private readonly IRolesRepository _rolesRepository;

        public RoleController(IRolesRepository RolesRepository, JwtTokenGenerator generateJwtToken)
        {
            _rolesRepository = RolesRepository;
            _generateJwtToken = generateJwtToken;
        }




        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _rolesRepository.GetAllUserRolesAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _rolesRepository.GetRoleByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(Role role)
        {
            var userId = await _rolesRepository.CreateRoleAsync(role);
            role.Id = userId;
            return CreatedAtAction(nameof(GetUser), new { id = role.Id }, role);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(Role role)
        {
            var updatedUser = await _rolesRepository.UpdateRoleAsync(role);

            if (updatedUser == null)
            {
                return NotFound();
            }

            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _rolesRepository.DeleteRoleAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
