using ProjectHierarchyApi.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectHierarchyApi.Services
{
    public class DeviceService
    {
        private readonly IMongoCollection<Device> _devices;
        private readonly IMongoCollection<Project> _projects;
        private readonly IMongoCollection<Location> _locations;
        private readonly IMongoCollection<Canteen> _canteens;

        public DeviceService(IMongoDatabase database)
        {
            _devices = database.GetCollection<Device>("Devices");
            _projects = database.GetCollection<Project>("Projects");
            _locations = database.GetCollection<Location>("Locations");
            _canteens = database.GetCollection<Canteen>("Canteens");
        }

        public async Task<List<Device>> GetDevicesAsync() => await _devices.Find(device => true).ToListAsync();

        public async Task<Device?> GetDeviceByUniqueIdAsync(string uniqueId) =>
            await _devices.Find(device => device.UniqueId == uniqueId).FirstOrDefaultAsync();

        public async Task<bool> CreateDeviceAsync(Device device)
        {
            var project = await _projects.Find(p => p.Id == device.ProjectId).FirstOrDefaultAsync();
            var location = await _locations.Find(l => l.Id == device.LocationId && l.ProjectId == device.ProjectId).FirstOrDefaultAsync();
            var canteen = await _canteens.Find(c => c.Id == device.CanteenId && c.LocationId == device.LocationId).FirstOrDefaultAsync();

            if (project == null || location == null || canteen == null)
                return false;

            // Prevent duplicate UniqueId entries
            var existingDevice = await GetDeviceByUniqueIdAsync(device.UniqueId);
            if (existingDevice != null)
                return false; 

            device.ProjectName = project.Name;
            device.LocationName = location.Name;
            device.CanteenName = canteen.Name;

            await _devices.InsertOneAsync(device);
            return true;
        }

        public async Task<bool> UpdateDeviceByUniqueIdAsync(string uniqueId, Device updatedDevice)
        {
            var existingDevice = await GetDeviceByUniqueIdAsync(uniqueId);
            if (existingDevice == null) return false;

            var project = await _projects.Find(p => p.Id == updatedDevice.ProjectId).FirstOrDefaultAsync();
            var location = await _locations.Find(l => l.Id == updatedDevice.LocationId && l.ProjectId == updatedDevice.ProjectId).FirstOrDefaultAsync();
            var canteen = await _canteens.Find(c => c.Id == updatedDevice.CanteenId && c.LocationId == updatedDevice.LocationId).FirstOrDefaultAsync();

            if (project == null || location == null || canteen == null)
                return false;

            updatedDevice.ProjectName = project.Name;
            updatedDevice.LocationName = location.Name;
            updatedDevice.CanteenName = canteen.Name;

            var result = await _devices.ReplaceOneAsync(device => device.UniqueId == uniqueId, updatedDevice);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteDeviceByUniqueIdAsync(string uniqueId)
        {
            var result = await _devices.DeleteOneAsync(device => device.UniqueId == uniqueId);
            return result.DeletedCount > 0;
        }
    }
}
