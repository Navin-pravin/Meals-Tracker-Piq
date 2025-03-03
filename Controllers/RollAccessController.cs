using Microsoft.AspNetCore.Mvc;
using AljasAuthApi.Services;
using AljasAuthApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AljasAuthApi.Controllers
{
    [Route("api/roleaccess")]
    [ApiController]
    public class RoleAccessController : ControllerBase
    {
        private readonly RoleAccessService _roleAccessService;
        private readonly UserService _userService;

        public RoleAccessController(RoleAccessService roleAccessService, UserService userService)
        {
            _roleAccessService = roleAccessService;
            _userService = userService;
        }

        // ✅ Add a new role with permissions
        [HttpPost("add")]
        public async Task<IActionResult> AddRole([FromBody] RoleAccess1 roleAccess)
        {
            if (roleAccess == null || string.IsNullOrEmpty(roleAccess.RoleName))
                return BadRequest(new { message = "Invalid role data" });

            await _roleAccessService.AddRoleAccessAsync(roleAccess);
            return Ok(new { message = "Role added successfully" });
        }

        // ✅ Get all roles and their permissions
        [HttpGet("list")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleAccessService.GetAllRolesAsync();
            return Ok(roles);
        }

        // ✅ Update Role Access and Sync Users
        // [HttpPut("update-role/{oldRoleName}")]
        // public async Task<IActionResult> UpdateRoleAccess(string oldRoleName, [FromBody] RoleAccess1 updatedRole)
        // {
        //     if (updatedRole == null || string.IsNullOrEmpty(updatedRole.RoleName))
        //         return BadRequest(new { message = "Invalid role data" });

        //     bool updated = await _roleAccessService.UpdateRoleAccessAsync(oldRoleName, updatedRole);
        //     if (!updated)
        //         return NotFound(new { message = "Role not found or update failed" });

        //     // ✅ Sync all users with the updated RoleName and RoleAccess
        //     await _userService.SyncUserRoleAccessAsync(oldRoleName, updatedRole);

        //     return Ok(new { message = "Role updated successfully" });
        // }

        // ✅ Check if a user role has access to a module
        [HttpGet("check-access")]
        public async Task<IActionResult> CheckAccess([FromQuery] string roleName, [FromQuery] string moduleName)
        {
            var hasAccess = await _roleAccessService.HasAccessAsync(roleName, moduleName);
            return Ok(new { roleName, moduleName, hasAccess });
        }
    }
}
