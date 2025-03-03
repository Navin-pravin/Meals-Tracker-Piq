using AljasAuthApi.Config;
using AljasAuthApi.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AljasAuthApi.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly RedisService _redisService;
        private readonly RoleAccessService _roleAccessService;

        public UserService(MongoDbSettings dbSettings, RedisService redisService, RoleAccessService roleAccessService)
        {
            var client = new MongoClient(dbSettings.ConnectionString);
            var database = client.GetDatabase(dbSettings.DatabaseName);
            _users = database.GetCollection<User>("Users");
            _redisService = redisService;
            _roleAccessService = roleAccessService;
        }

        // ✅ Get User Summary
        public async Task<List<UserSummary>> GetUserSummaryAsync()
        {
            return await _users.Find(_ => true)
                .Project(u => new UserSummary
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    ContactNo = u.ContactNo,
                    RoleName = u.RoleName,
                    RoleAccess = u.RoleAccess
                })
                .ToListAsync();
        }

        // ✅ Create User with Role Sync
        public async Task CreateUserAsync(CreateUserRequest request)
        {
            var roleAccess = await _roleAccessService.GetRoleAccessAsync(request.RoleName);
            if (roleAccess == null)
                throw new Exception("Invalid Role. Cannot create user.");

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                ContactNo = request.ContactNo,
                Password = request.Password,
                RoleName = request.RoleName,
                RoleAccess = roleAccess.RoleAccess // ✅ Auto-sync RoleAccess
            };

            await _users.InsertOneAsync(user);
            await _redisService.PublishEventAsync("UserCreated", user);
        }

        // ✅ Update User & Sync RoleAccess
        public async Task<bool> UpdateUserAsync(UpdateUserRequest request)
        {
            var update = Builders<User>.Update
                .Set(u => u.Username, request.Username)
                .Set(u => u.Email, request.Email)
                .Set(u => u.ContactNo, request.ContactNo)
                .Set(u => u.RoleName, request.RoleName);

            var roleAccess = await _roleAccessService.GetRoleAccessAsync(request.RoleName);
            if (roleAccess != null)
            {
                update = update.Set(u => u.RoleAccess, roleAccess.RoleAccess);
            }

            if (!string.IsNullOrEmpty(request.Password))
            {
                update = update.Set(u => u.Password, request.Password);
            }

            var result = await _users.UpdateOneAsync(u => u.Id == request.Id, update);
            return result.ModifiedCount > 0;
        }

        // ✅ Delete User
        public async Task<bool> DeleteUserAsync(string id)
        {
            var result = await _users.DeleteOneAsync(u => u.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
