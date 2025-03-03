using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace AljasAuthApi.Models
{
    public class RoleAccess1
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }// = ObjectId.GenerateNewId().ToString();

        [BsonElement("RoleName")] // ✅ Matches MongoDB field name
        public required string RoleName { get; set; } // e.g., "SuperAdmin", "Admin", "User"

        [BsonElement("RoleAccess")] // ✅ Ensures consistent naming across collections
        public required List<string> RoleAccess { get; set; } = new(); // List of modules user can access
    }
}
