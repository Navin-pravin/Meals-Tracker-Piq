using Microsoft.AspNetCore.Mvc;
using AljasAuthApi.Models;
using AljasAuthApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using OfficeOpenXml;

namespace AljasAuthApi.Controllers
{
    [Route("api/sub-contractors")]
    [ApiController]
    public class SubContractorController : ControllerBase
    {
        private readonly SubContractorService _subContractorService;

        public SubContractorController(SubContractorService subContractorService)
        {
            _subContractorService = subContractorService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetAllSubContractors([FromQuery] string? name = null, [FromQuery] string? company = null)
        {
            var subContractors = await _subContractorService.GetAllSubContractorsAsync(name, company);
            return Ok(subContractors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubContractorById(string id)
        {
            var subContractor = await _subContractorService.GetSubContractorByIdAsync(id);
            if (subContractor == null)
                return NotFound(new { message = "SubContractor not found" });

            return Ok(subContractor);
        }

        [HttpPost("add")]
        public async Task<IActionResult> CreateSubContractor([FromBody] SubContractor? subContractor)
        {
            if (subContractor == null)
                return BadRequest(new { message = "Invalid subcontractor data." });

            bool created = await _subContractorService.CreateSubContractorAsync(subContractor);
            if (!created)
                return StatusCode(500, new { message = "Failed to create subcontractor." });

            return CreatedAtAction(nameof(GetSubContractorById), new { id = subContractor.Id }, subContractor);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateSubContractor(string id, [FromBody] SubContractor? updatedSubContractor)
        {
            if (updatedSubContractor == null)
                return BadRequest(new { message = "Invalid subcontractor data." });

            bool success = await _subContractorService.UpdateSubContractorAsync(id, updatedSubContractor);
            if (!success)
                return NotFound(new { message = "SubContractor not found" });

            return Ok(new { message = "SubContractor updated successfully" });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSubContractor(string id)
        {
            bool success = await _subContractorService.DeleteSubContractorAsync(id);
            if (!success)
                return NotFound(new { message = "SubContractor not found" });

            return Ok(new { message = "SubContractor deleted successfully" });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadSubContractors(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Invalid file." });

            bool success = await _subContractorService.UploadSubContractorsAsync(file);
            if (!success)
                return StatusCode(500, new { message = "Failed to upload subcontractors." });

            return Ok(new { message = "SubContractors uploaded successfully." });
        }

        // ✅ Sample Excel Download API (No async needed)
        [HttpGet("download-sample")]
        public IActionResult DownloadSampleExcel() // Removed async to fix warning CS1998
        {
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("SubContractor Template");

                // ✅ Add headers
                worksheet.Cells[1, 1].Value = "ContractorName";
                worksheet.Cells[1, 2].Value = "ContractorId";
                worksheet.Cells[1, 3].Value = "CompanyName";
                worksheet.Cells[1, 4].Value = "ProjectName";
                worksheet.Cells[1, 5].Value = "Address";
                worksheet.Cells[1, 6].Value = "PhoneNo";
                worksheet.Cells[1, 7].Value = "Nationality";
                worksheet.Cells[1, 8].Value = "VehicleName";
                worksheet.Cells[1, 9].Value = "VehicleId";
                worksheet.Cells[1, 10].Value = "ImageUrl";

                // ✅ Auto-fit columns for better readability
                worksheet.Cells.AutoFitColumns();

                // ✅ Save the Excel package
                package.Save();
            }

            stream.Position = 0;
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = "SubContractor_BulkUpload_Template.xlsx";

            return File(stream, contentType, fileName);
        }
    }
}
