using AljasAuthApi.Models;
using MongoDB.Driver;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AljasAuthApi.Services
{
    public class VisitorService
    {
        private readonly IMongoCollection<Visitor> _visitors;
        private readonly RabbitMQService _rabbitMQService;

        public VisitorService(IMongoDatabase database, RabbitMQService rabbitMQService)
        {
            _visitors = database.GetCollection<Visitor>("Visitors");
            _rabbitMQService = rabbitMQService;
        }

        // ✅ Create Visitor & Send Event
        public async Task CreateVisitorAsync(Visitor visitor)
        {
            await _visitors.InsertOneAsync(visitor);
            PublishEvent("visitor.created", visitor);
        }

        // ✅ Update Visitor & Send Event
        public async Task<bool> UpdateVisitorAsync(string id, Visitor updatedVisitor)
        {
            var result = await _visitors.ReplaceOneAsync(v => v.Id == id, updatedVisitor);
            if (result.ModifiedCount > 0)
            {
                PublishEvent("visitor.updated", updatedVisitor);
                return true;
            }
            return false;
        }

        // ✅ Delete Visitor & Send Event
        public async Task<bool> DeleteVisitorAsync(string id)
        {
            var result = await _visitors.DeleteOneAsync(v => v.Id == id);
            if (result.DeletedCount > 0)
            {
                PublishEvent("visitor.deleted", new { Id = id });
                return true;
            }
            return false;
        }

        // ✅ Get Visitor by ID
        public async Task<Visitor?> GetVisitorByIdAsync(string id)
        {
            return await _visitors.Find(v => v.Id == id).FirstOrDefaultAsync();
        }

        // ✅ Get Visitor Summary
        public async Task<List<Visitor>> GetVisitorSummaryAsync()
        {
            return await _visitors.Find(_ => true).ToListAsync();
        }

        // ✅ Bulk Upload Visitors & Publish Events
        public async Task<bool> BulkUploadVisitorsAsync(Stream fileStream)
        {
            try
            {
                using var package = new ExcelPackage(fileStream);
                var worksheet = package.Workbook.Worksheets[0];

                var visitors = new List<Visitor>();
                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {
                    if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text)) continue; // Skip empty rows

                    if (!DateTime.TryParse(worksheet.Cells[row, 3].Text, out DateTime startDate) ||
                        !DateTime.TryParse(worksheet.Cells[row, 4].Text, out DateTime endDate))
                    {
                        continue; // Skip rows with invalid dates
                    }

                    var visitor = new Visitor
                    {
                        VisitorName = worksheet.Cells[row, 1].Text,
                        Email = worksheet.Cells[row, 2].Text,
                        StartDate = startDate,
                        EndDate = endDate,
                        VisitorCompany = worksheet.Cells[row, 5].Text,
                        ContactNo = worksheet.Cells[row, 6].Text
                    };

                    visitors.Add(visitor);
                }

                if (visitors.Count > 0)
                {
                    // 🔹 Insert visitors in Batches
                    int batchSize = 500; // Adjust for MongoDB performance
                    for (int i = 0; i < visitors.Count; i += batchSize)
                    {
                        var batch = visitors.Skip(i).Take(batchSize).ToList();
                        await _visitors.InsertManyAsync(batch);

                        // 🔹 Publish each visitor event
                        foreach (var visitor in batch)
                        {
                            PublishEvent("visitor.created", visitor);
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Bulk Upload Error] {ex.Message}");
            }

            return false;
        }

        // ✅ Generate Sample Excel for Visitor Upload
        public byte[] GenerateSampleExcel()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Visitors");

            // Header Row
            worksheet.Cells[1, 1].Value = "VisitorName";
            worksheet.Cells[1, 2].Value = "Email";
            worksheet.Cells[1, 3].Value = "StartDate";
            worksheet.Cells[1, 4].Value = "EndDate";
            worksheet.Cells[1, 5].Value = "VisitorCompany";
            worksheet.Cells[1, 6].Value = "ContactNo";

            return package.GetAsByteArray();
        }

        // ✅ Centralized Event Publisher with Exception Handling
        private void PublishEvent(string routingKey, object message)
        {
            try
            {
                _rabbitMQService.PublishMessage(routingKey, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RabbitMQ Error] Failed to publish event '{routingKey}': {ex.Message}");
            }
        }
    }
}
