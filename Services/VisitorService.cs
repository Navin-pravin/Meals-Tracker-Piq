using AljasAuthApi.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AljasAuthApi.Services
{
    public class VisitorService
    {
        private readonly IMongoCollection<Visitor> _visitors;

        public VisitorService(IMongoDatabase database)
        {
            _visitors = database.GetCollection<Visitor>("Visitors");
        }

        // ✅ Create Visitor
        public async Task CreateVisitorAsync(Visitor visitor)
        {
            await _visitors.InsertOneAsync(visitor);
        }

        // ✅ Get Visitor by ID
        public async Task<Visitor?> GetVisitorByIdAsync(string id)
        {
            return await _visitors.Find(v => v.Id == id).FirstOrDefaultAsync();
        }

        // ✅ Get All Visitors (Summary)
        public async Task<List<Visitor>> GetVisitorSummaryAsync()
        {
            return await _visitors.Find(_ => true).ToListAsync();
        }

        // ✅ Update Visitor
        public async Task<bool> UpdateVisitorAsync(string id, Visitor updatedVisitor)
        {
            var update = Builders<Visitor>.Update
                .Set(v => v.VisitorName, updatedVisitor.VisitorName)
                .Set(v => v.Email, updatedVisitor.Email)
                .Set(v => v.StartDate, updatedVisitor.StartDate)
                .Set(v => v.EndDate, updatedVisitor.EndDate)
                .Set(v => v.VisitorCompany, updatedVisitor.VisitorCompany)
                .Set(v => v.ContactNo, updatedVisitor.ContactNo);

            var result = await _visitors.UpdateOneAsync(v => v.Id == id, update);
            return result.ModifiedCount > 0;
        }

        // ✅ Delete Visitor
        public async Task<bool> DeleteVisitorAsync(string id)
        {
            var result = await _visitors.DeleteOneAsync(v => v.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
