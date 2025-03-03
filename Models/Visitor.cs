using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AljasAuthApi.Models
{
    public class Visitor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public required string VisitorName { get; set; }
        public required string Email { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required string VisitorCompany { get; set; }
        public required string ContactNo { get; set; }
    }
}
