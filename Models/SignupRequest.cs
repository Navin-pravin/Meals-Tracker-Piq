using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace AljasAuthApi.Models

{
    public class SignupRequest
    { [BsonId]
        [BsonRepresentation(BsonType.String)]
       //ublic string Id { get; set; } = string.Empty;
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}