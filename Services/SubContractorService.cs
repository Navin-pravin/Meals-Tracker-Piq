using AljasAuthApi.Config;
using AljasAuthApi.Models;
using MongoDB.Driver;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AljasAuthApi.Services
{
    public class SubContractorService
    {
        private readonly IMongoCollection<SubContractor> _bulkSubContractors;

        public SubContractorService(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _bulkSubContractors = database.GetCollection<SubContractor>("Sub-contractors-bulk");
        }

        public async Task<List<SubContractor>> GetAllSubContractorsAsync(string? name = null, string? company = null)
        {
            var filter = Builders<SubContractor>.Filter.Empty;

            if (!string.IsNullOrEmpty(name))
                filter &= Builders<SubContractor>.Filter.Regex("contractor_name", new MongoDB.Bson.BsonRegularExpression(name, "i"));

            if (!string.IsNullOrEmpty(company))
                filter &= Builders<SubContractor>.Filter.Eq(sc => sc.CompanyName, company);

            return await _bulkSubContractors.Find(filter).ToListAsync();
        }

        public async Task<SubContractor?> GetSubContractorByIdAsync(string id) =>
            await _bulkSubContractors.Find(sc => sc.Id == id).FirstOrDefaultAsync();

        public async Task<bool> CreateSubContractorAsync(SubContractor subContractor)
        {
            if (subContractor == null) return false;

            subContractor.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            await _bulkSubContractors.InsertOneAsync(subContractor);
            return true;
        }

        public async Task<bool> UpdateSubContractorAsync(string id, SubContractor updatedSubContractor)
        {
            var result = await _bulkSubContractors.ReplaceOneAsync(sc => sc.Id == id, updatedSubContractor);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteSubContractorAsync(string id)
        {
            var result = await _bulkSubContractors.DeleteOneAsync(sc => sc.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> UploadSubContractorsAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];

            var subContractors = new List<SubContractor>();

            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                var subContractor = new SubContractor
                {
                    Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    ContractorName = worksheet.Cells[row, 1].Text.Trim(),
                    CompanyName = worksheet.Cells[row, 2].Text.Trim()
                };

                subContractors.Add(subContractor);
            }

            if (subContractors.Count > 0)
            {
                await _bulkSubContractors.InsertManyAsync(subContractors);
                return true;
            }

            return false;
        }
    }
}
