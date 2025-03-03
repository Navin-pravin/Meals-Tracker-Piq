using AljasAuthApi.Models;

using AljasAuthApi.Config;

using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AljasAuthApi.Services
{
    public class RoleAccessService
    {
        private readonly IMongoCollection<RoleAccess1>_roles;
        public readonly IMongoCollection<User> _users;

        public RoleAccessService(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<User>("Users");
            _roles = database.GetCollection<RoleAccess1>("RoleAccess");
        }

        public async Task AddRoleAccessAsync(RoleAccess1 roleAccess)
        {
            await _roles.InsertOneAsync(roleAccess);
        }
        public async Task<List<RoleAccess1>>GetAllRolesAsync()
        {
            return await _roles.Find(_ => true).ToListAsync();
        }

        public async Task<RoleAccess1?>GetRoleAccessAsync(string roleName)
        {
            return await _roles.Find(r => r.RoleName == roleName).FirstOrDefaultAsync();
        }

        public async Task<bool> HasAccessAsync(string roleName, string moduleName)
        {
            // ✅ Step 1: Check if any user with the given RoleName has access to the module
            var userHasAccess = await _users
                .Find(u => u.RoleName == roleName && u.RoleAccess.Contains(moduleName))
                .AnyAsync();

            // ✅ Step 2: Check if the RoleAccess collection allows the module
            var roleAccess = await GetRoleAccessAsync(roleName);
            var roleHasAccess = roleAccess != null && roleAccess.RoleAccess.Contains(moduleName);

            // ✅ Step 3: Return true if either condition is satisfied
            return userHasAccess || roleHasAccess;
        }
    }
}
