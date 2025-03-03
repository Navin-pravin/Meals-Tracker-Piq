using Microsoft.AspNetCore.Mvc;
using ProjectHierarchyApi.Models;
using ProjectHierarchyApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectHierarchyApi.Controllers
{
    [Route("api/locations")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly LocationService _locationService;

        public LocationController(LocationService locationService)
        {
            _locationService = locationService;
        }

        // ✅ Get all locations for a given project
        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<List<Location>>> GetLocationsByProject(string projectId)
        {
            var locations = await _locationService.GetLocationsByProjectIdAsync(projectId);
            return Ok(locations);
        }

        // ✅ Create a new location under a project
        [HttpPost]
        public async Task<IActionResult> CreateLocation(Location location)
        {
            await _locationService.CreateLocationAsync(location);
            return Ok(new { message = "Location created successfully" });
        }

        // ✅ Update a location
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(string id, Location updatedLocation)
        {
            var success = await _locationService.UpdateLocationAsync(id, updatedLocation);
            if (!success) return NotFound(new { message = "Location not found" });
            return Ok(new { message = "Location updated successfully" });
        }

        // ✅ Delete a location
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(string id)
        {
            var success = await _locationService.DeleteLocationAsync(id);
            if (!success) return NotFound(new { message = "Location not found" });
            return Ok(new { message = "Location deleted successfully" });
        }
    }
}
