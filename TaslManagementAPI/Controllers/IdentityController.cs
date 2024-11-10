using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaslManagementAPI.DTOs;
using TaslManagementAPI.Models;
using TaslManagementAPI.Utilities;

namespace TaslManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("AssignRole")]
        [Authorize(Roles = RolesConstants.Admin)]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return NotFound($"User with username '{model.Username}' not found.");
            }

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                return BadRequest($"Role '{model.Role}' does not exist.");
            }

            var result = await _userManager.AddToRoleAsync(user, model.Role);
            if (result.Succeeded)
            {
                return Ok($"Role '{model.Role}' assigned to user '{model.Username}'.");
            }

            return BadRequest("Failed to assign role.");
        }
        [HttpPost("RemoveRole")]
        [Authorize(Roles = RolesConstants.Admin)]
        public async Task<IActionResult> RemoveRole([FromBody] AssignRoleDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return NotFound($"User with username '{model.Username}' not found.");
            }

            var result = await _userManager.RemoveFromRoleAsync(user, model.Role);
            if (result.Succeeded)
            {
                return Ok($"Role '{model.Role}' removed from user '{model.Username}'.");
            }

            return BadRequest("Failed to remove role.");
        }
        [HttpGet("GetUserRoles/{username}")]
        [Authorize(Roles = RolesConstants.Admin)]
        public async Task<IActionResult> GetUserRoles(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound($"User with username '{username}' not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersWithRoles = new List<dynamic>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add(new
                {
                    Username = user.UserName,
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = roles
                });
            }
            return Ok(usersWithRoles);
        }
    }
}