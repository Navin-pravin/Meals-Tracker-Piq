using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace AljasAuthApi.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("Username")]
        public required string Username { get; set; }

        [BsonElement("Email")]
        public required string Email { get; set; }

        [BsonElement("ContactNo")]
        public required string ContactNo { get; set; }

        [BsonElement("Password")]
        public required string Password { get; set; }

        [BsonElement("RoleName")] // ✅ Ensures consistency across user & role
        public required string RoleName { get; set; } // e.g., "SuperAdmin", "Admin", "User"

        [BsonElement("RoleAccess")]
        public required List<string> RoleAccess { get; set; } = new(); // Modules user can access

        public string? OTP { get; set; }
        public DateTime? OTPGeneratedAt { get; set; }
    }
}
