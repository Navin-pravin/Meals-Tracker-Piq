using MongoDB.Driver;
using ProjectHierarchyApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectHierarchyApi.Services
{
    public class LocationService
    {
        private readonly IMongoCollection<Location> _locations;

        public LocationService(IMongoDatabase database)
        {
            _locations = database.GetCollection<Location>("Locations");
        }

        public async Task<List<Location>> GetLocationsByProjectIdAsync(string projectId) =>
            await _locations.Find(l => l.ProjectId == projectId).ToListAsync();

        public async Task CreateLocationAsync(Location location) =>
            await _locations.InsertOneAsync(location);

        public async Task<bool> UpdateLocationAsync(string id, Location updatedLocation)
        {
            var result = await _locations.ReplaceOneAsync(l => l.Id == id, updatedLocation);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteLocationAsync(string id)
        {
            var result = await _locations.DeleteOneAsync(l => l.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
