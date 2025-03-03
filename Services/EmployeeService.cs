using AljasAuthApi.Config;
using AljasAuthApi.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MongoDB.Bson;

namespace AljasAuthApi.Services
{
    public class EmployeeService
    {
        private readonly IMongoCollection<Employee> _employees;

        public EmployeeService(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _employees = database.GetCollection<Employee>(settings.EmployeesCollectionName);
        }

        // ✅ Get all employees with optional filters (Firstname & Department)
        public async Task<List<Employee>> GetAllEmployeesAsync(string? Firstname = null, string? Dept = null)
        {
            var filter = Builders<Employee>.Filter.Empty;

            if (!string.IsNullOrEmpty(Firstname))
                filter &= Builders<Employee>.Filter.Regex("Firstname", new BsonRegularExpression(Firstname, "i"));

            if (!string.IsNullOrEmpty(Dept))
                filter &= Builders<Employee>.Filter.Eq(emp => emp.Dept, Dept);

            return await _employees.Find(filter).ToListAsync();
        }

        // ✅ Get a specific employee by ID
        public async Task<Employee?> GetEmployeeByIdAsync(string id) =>
            await _employees.Find(emp => emp.Id == id).FirstOrDefaultAsync();

        // ✅ Add a new employee
        public async Task<bool> CreateEmployeeAsync(Employee employee)
        {
            if (employee == null) return false;
            employee.Id = Guid.NewGuid().ToString(); // Ensuring Id is string
            try
            {
                await _employees.InsertOneAsync(employee);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to create employee: {ex.Message}");
                return false;
            }
        }

        // ✅ Update an existing employee
        public async Task<bool> UpdateEmployeeAsync(string id, Employee updatedEmployee)
        {
            var result = await _employees.ReplaceOneAsync(emp => emp.Id == id, updatedEmployee);
            return result.ModifiedCount > 0;
        }

        // ✅ Delete an employee by ID
        public async Task<bool> DeleteEmployeeAsync(string id)
        {
            var result = await _employees.DeleteOneAsync(emp => emp.Id == id);
            return result.DeletedCount > 0;
        }

        // ✅ Bulk Upload Employees from Excel
        public async Task<bool> BulkUploadEmployeesFromExcelAsync(List<Employee> employees)
        {
            if (employees == null || employees.Count == 0)
            {
                Console.WriteLine("❌ Bulk upload failed: No employees found.");
                return false;
            }

            // Ensure unique IDs for each employee
            foreach (var employee in employees)
            {
                employee.Id = employee.Id ?? Guid.NewGuid().ToString();
            }

            try
            {
                await _employees.InsertManyAsync(employees);
                Console.WriteLine($"✅ Successfully inserted {employees.Count} employees.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Bulk insert failed: {ex.Message}");
                return false;
            }
        }
    }
}
