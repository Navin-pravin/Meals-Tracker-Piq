using Microsoft.AspNetCore.Mvc;
using AljasAuthApi.Models;
using AljasAuthApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using OfficeOpenXml;
using System.IO;

namespace AljasAuthApi.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeService _employeeService;

        public EmployeeController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("Employee-Summary")]
        public async Task<IActionResult> GetAllEmployees([FromQuery] string? Firstname = null, [FromQuery] string? Dept = null)
        {
            var employees = await _employeeService.GetAllEmployeesAsync(Firstname, Dept);
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(string id)
        {
    var employee = await _employeeService.GetEmployeeByIdAsync(id);
    if (employee == null)
        return NotFound(new { message = "Employee not found" });

    // ✅ Restrict access if employee status is "Inactive"
    if (employee.Status.ToLower() == "inactive")
        return BadRequest(new { message = "Access denied. Employee is inactive." });

    return Ok(employee);
}


        [HttpPost("add-employee")]
        public async Task<IActionResult> CreateEmployee([FromBody] Employee? employee)
        {
            if (employee == null)
                return BadRequest(new { message = "Invalid employee data." });

            bool created = await _employeeService.CreateEmployeeAsync(employee);
            if (!created)
                return StatusCode(500, new { message = "Failed to create employee." });

            return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
        }

        [HttpPut("update-employee/{id}")]
        public async Task<IActionResult> UpdateEmployee(string id, [FromBody] Employee? updatedEmployee)
        {
            if (updatedEmployee == null)
                return BadRequest(new { message = "Invalid employee data." });

            bool success = await _employeeService.UpdateEmployeeAsync(id, updatedEmployee);
            if (!success)
                return NotFound(new { message = "Employee not found" });

            return Ok(new { message = "Employee updated successfully" });
        }

        [HttpDelete("delete-employee/{id}")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            bool success = await _employeeService.DeleteEmployeeAsync(id);
            if (!success)
                return NotFound(new { message = "Employee not found" });

            return Ok(new { message = "Employee deleted successfully" });
        }
        [HttpPost("upload-excel")]
public async Task<IActionResult> UploadEmployeesFromExcel(IFormFile file)
{
    if (file == null || file.Length == 0)
        return BadRequest(new { message = "No file uploaded." });

    if (file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        return BadRequest(new { message = "Invalid file format. Please upload a valid Excel file." });

    try
    {
        var employees = new List<Employee>();

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension?.Rows ?? 0;

                if (rowCount < 2)
                    return BadRequest(new { message = "Excel file is empty or missing headers." });

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var employee = new Employee
                        {
                            Id = Guid.NewGuid().ToString(),
                            IDNumber = worksheet.Cells[row, 1]?.Text?.Trim() ?? "",
                            Firstname = worksheet.Cells[row, 2]?.Text?.Trim() ?? "",
                            Lastname = worksheet.Cells[row, 3]?.Text?.Trim() ?? "",
                            StartDate = worksheet.Cells[row, 4]?.Text?.Trim() ?? "",
                            EndDate = worksheet.Cells[row, 5]?.Text?.Trim() ?? "",
                            Phone_no = worksheet.Cells[row, 6]?.Text?.Trim() ?? "",
                            Dept = worksheet.Cells[row, 7]?.Text?.Trim() ?? "",
                            Role = worksheet.Cells[row, 8]?.Text?.Trim() ?? "",
                            designation = worksheet.Cells[row, 9]?.Text?.Trim() ?? "",
                            imageUrl = worksheet.Cells[row, 10]?.Text?.Trim() ?? "",
                            Company = worksheet.Cells[row, 11]?.Text?.Trim() ?? "",
                            Referenceid = worksheet.Cells[row, 12]?.Text?.Trim() ?? "",
                            CardBadgeNumber = worksheet.Cells[row, 13]?.Text?.Trim() ?? "",
                            Status = worksheet.Cells[row, 14]?.Text?.Trim() ?? "Inactive" // Default to "Inactive"
                        };

                        employees.Add(employee);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error processing row {row}: {ex.Message}");
                    }
                }
            }
        }

        bool uploaded = await _employeeService.BulkUploadEmployeesFromExcelAsync(employees);
        if (!uploaded)
            return StatusCode(500, new { message = "Failed to upload employees. Check server logs." });

        return Ok(new { message = $"{employees.Count} employees uploaded successfully." });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Upload failed: {ex}");
        return StatusCode(500, new { message = "Error processing the Excel file.", details = ex.Message });
    }
}


        // ✅ Sample Excel Download API (No async needed)
        [HttpGet("download-sample")]
public IActionResult DownloadSampleExcel()
{
    var stream = new MemoryStream();

    using (var package = new ExcelPackage(stream))
    {
        var worksheet = package.Workbook.Worksheets.Add("Employee Template");

        // ✅ Add headers
        worksheet.Cells[1, 1].Value = "IDNumber";
        worksheet.Cells[1, 2].Value = "Firstname";
        worksheet.Cells[1, 3].Value = "Lastname";
        worksheet.Cells[1, 4].Value = "StartDate";
        worksheet.Cells[1, 5].Value = "EndDate";
        worksheet.Cells[1, 6].Value = "Phone_no";
        worksheet.Cells[1, 7].Value = "Dept";
        worksheet.Cells[1, 8].Value = "Role";
        worksheet.Cells[1, 9].Value = "designation";
        worksheet.Cells[1, 10].Value = "imageUrl";
        worksheet.Cells[1, 11].Value = "Company";
        worksheet.Cells[1, 12].Value = "Referenceid";
        worksheet.Cells[1, 13].Value = "CardBadgeNumber";
        worksheet.Cells[1, 14].Value = "Status"; // ✅ New field

        // ✅ Auto-fit columns for better readability
        worksheet.Cells.AutoFitColumns();

        // ✅ Save the Excel package
        package.Save();
    }

    stream.Position = 0;
    var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    var fileName = "Employee_BulkUpload_Template.xlsx";

    return File(stream, contentType, fileName);
}
    }}