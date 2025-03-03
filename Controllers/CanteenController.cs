using Microsoft.AspNetCore.Mvc;
using ProjectHierarchyApi.Models;
using ProjectHierarchyApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectHierarchyApi.Controllers
{
    [Route("api/canteens")]
    [ApiController]
    public class CanteenController : ControllerBase
    {
        private readonly CanteenService _canteenService;

        public CanteenController(CanteenService canteenService)
        {
            _canteenService = canteenService;
        }

        // ✅ Get all canteens for a given project and location
        [HttpGet("project/{projectId}/location/{locationId}")]
        public async Task<ActionResult<List<Canteen>>> GetCanteensByLocation(string projectId, string locationId)
        {
            var canteens = await _canteenService.GetCanteensByLocationIdAsync(locationId);
            return Ok(canteens);
        }

        // ✅ Create a new canteen under a project and location
        [HttpPost]
        public async Task<IActionResult> CreateCanteen(Canteen canteen)
        {
            await _canteenService.CreateCanteenAsync(canteen);
            return Ok(new { message = "Canteen created successfully" });
        }

        // ✅ Update a canteen
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCanteen(string id, Canteen updatedCanteen)
        {
            var success = await _canteenService.UpdateCanteenAsync(id, updatedCanteen);
            if (!success) return NotFound(new { message = "Canteen not found" });
            return Ok(new { message = "Canteen updated successfully" });
        }

        // ✅ Delete a canteen
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCanteen(string id)
        {
            var success = await _canteenService.DeleteCanteenAsync(id);
            if (!success) return NotFound(new { message = "Canteen not found" });
            return Ok(new { message = "Canteen deleted successfully" });
        }
    }
}
