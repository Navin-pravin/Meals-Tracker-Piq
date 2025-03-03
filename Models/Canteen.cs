using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectHierarchyApi.Models
{
    public class Canteen
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string Name { get; set; } = string.Empty;
        public string ProjectId { get; set; } = string.Empty;
        public string LocationId { get; set; } = string.Empty;
    }
}
