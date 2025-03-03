using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectHierarchyApi.Models
{
    public class Device
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string? Id { get; set; }

        [BsonElement("deviceName")]
        public string DeviceName { get; set; } = string.Empty;

        [BsonElement("uniqueId")]
        public string UniqueId { get; set; } = string.Empty;

        [BsonElement("projectId")]
        [BsonRepresentation(BsonType.String)]
        public string ProjectId { get; set; } = string.Empty;

        [BsonElement("projectName")]
        public string ProjectName { get; set; } = string.Empty;

        [BsonElement("locationId")]
        [BsonRepresentation(BsonType.String)]
        public string LocationId { get; set; } = string.Empty;

        [BsonElement("locationName")]
        public string LocationName { get; set; } = string.Empty;

        [BsonElement("canteenId")]
        [BsonRepresentation(BsonType.String)]
        public string CanteenId { get; set; } = string.Empty;

        [BsonElement("canteenName")]
        public string CanteenName { get; set; } = string.Empty;
    }
}
