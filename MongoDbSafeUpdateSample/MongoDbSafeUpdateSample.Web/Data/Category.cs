using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDbSafeUpdateSample.Web.Data
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime LastUpdatedOn { get; set; } // Yes, hate it but MongoDB C# Driver is not good at DateTimeOffsets :s
        public IEnumerable<Image> Images { get; set; }
    }
}