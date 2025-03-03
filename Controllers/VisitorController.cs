using AljasAuthApi.Services;
using AljasAuthApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AljasAuthApi.Controllers
{
    [Route("api/visitors")]
    [ApiController]
    public class VisitorController : ControllerBase
    {
        private readonly VisitorService _visitorService;

        public VisitorController(VisitorService visitorService)
        {
            _visitorService = visitorService;
        }

        // ✅ Create Visitor
        [HttpPost("create")]
        public async Task<IActionResult> CreateVisitor([FromBody] Visitor visitor)
        {
            if (visitor == null)
                return BadRequest(new { message = "Invalid visitor data" });

            await _visitorService.CreateVisitorAsync(visitor);
            return Ok(new { message = "Visitor created successfully" });
        }

        // ✅ Get Visitor by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVisitorById(string id)
        {
            var visitor = await _visitorService.GetVisitorByIdAsync(id);
            if (visitor == null)
                return NotFound(new { message = "Visitor not found" });

            return Ok(visitor);
        }

        // ✅ Get All Visitors (Summary)
        [HttpGet("summary")]
        public async Task<IActionResult> GetVisitorSummary()
        {
            var visitors = await _visitorService.GetVisitorSummaryAsync();
            return Ok(visitors);
        }

        // ✅ Update Visitor
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateVisitor(string id, [FromBody] Visitor visitor)
        {
            if (visitor == null)
                return BadRequest(new { message = "Invalid visitor data" });

            var isUpdated = await _visitorService.UpdateVisitorAsync(id, visitor);
            if (!isUpdated)
                return NotFound(new { message = "Visitor not found" });

            return Ok(new { message = "Visitor updated successfully" });
        }

        // ✅ Delete Visitor
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteVisitor(string id)
        {
            var isDeleted = await _visitorService.DeleteVisitorAsync(id);
            if (!isDeleted)
                return NotFound(new { message = "Visitor not found" });

            return Ok(new { message = "Visitor deleted successfully" });
        }
    }
}
