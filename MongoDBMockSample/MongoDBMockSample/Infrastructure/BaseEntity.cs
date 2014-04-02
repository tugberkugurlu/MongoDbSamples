using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBMockSample.Infrastructure {

    public abstract class BaseEntity {

        public BsonObjectId Id { get; set; }

        public string IdStr {
            get {
                return Id.ToString();
            }
        }

        public DateTime? DeletedOn { get; set; }
    }
}