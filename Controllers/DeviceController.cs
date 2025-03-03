using Microsoft.AspNetCore.Mvc;
using ProjectHierarchyApi.Models;
using ProjectHierarchyApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectHierarchyApi.Controllers
{
    [Route("api/devices")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly DeviceService _deviceService;

        public DeviceController(DeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetDevices()
        {
            var devices = await _deviceService.GetDevicesAsync();
            return Ok(devices);
        }

        // ✅ Get device by UniqueId
        [HttpGet("unique/{uniqueId}")]
        public async Task<IActionResult> GetDeviceByUniqueId(string uniqueId)
        {
            var device = await _deviceService.GetDeviceByUniqueIdAsync(uniqueId);
            if (device == null) return NotFound(new { message = "Device not found" });

            return Ok(device);
        }

        [HttpPost("add")]
        public async Task<IActionResult> CreateDevice([FromBody] Device device)
        {
            if (device == null) return BadRequest(new { message = "Invalid device data." });

            bool created = await _deviceService.CreateDeviceAsync(device);
            if (!created) return BadRequest(new { message = "Device with this UniqueId already exists or invalid project/location/canteen data." });

            return CreatedAtAction(nameof(GetDeviceByUniqueId), new { uniqueId = device.UniqueId }, device);
        }

        [HttpPut("update/{uniqueId}")]
        public async Task<IActionResult> UpdateDevice(string uniqueId, [FromBody] Device updatedDevice)
        {
            bool success = await _deviceService.UpdateDeviceByUniqueIdAsync(uniqueId, updatedDevice);
            if (!success) return NotFound(new { message = "Device not found or invalid project/location/canteen data." });

            return Ok(new { message = "Device updated successfully" });
        }

        [HttpDelete("delete/{uniqueId}")]
        public async Task<IActionResult> DeleteDevice(string uniqueId)
        {
            bool success = await _deviceService.DeleteDeviceByUniqueIdAsync(uniqueId);
            if (!success) return NotFound(new { message = "Device not found" });

            return Ok(new { message = "Device deleted successfully" });
        }
    }
}
