using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDbSafeUpdateSample.Web.Data
{
    public class CategoryReference
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}