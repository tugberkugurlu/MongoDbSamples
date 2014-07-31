using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace ConcurrencyUpdateSample
{
    class Program
    {
        static void Main(string[] args)
        {
            MongoClient client = new MongoClient();
            MongoServer server = client.GetServer();
            MongoDatabase database = server.GetDatabase("ConcurrencyUpdateSample");
            MongoCollection<Person> collection = database.GetCollection<Person>("people");

            Init(collection);
            Person person = Find(collection);
            person.Name = "Tugberk-2";
            new EntityReplaceUpdateCommand<Person>(collection, person).Execute();
        }

        static void Init(MongoCollection<Person> collection)
        {
            Person person = new Person
            {
                Name = "Tugberk",
                Surname = "Ugurlu",
                CreatedOn = DateTime.UtcNow
            };

            collection.Insert(person);
        }

        static Person Find(MongoCollection<Person> collection)
        {
            return collection.FindOne(Query<Person>.EQ(person => person.Name, "Tugberk"));
        }
    }

    public class EntityReplaceUpdateCommand<TEntity> where TEntity : class, IEntity
    {
        private readonly MongoCollection<TEntity> _collection;
        private readonly TEntity _entity;
        private readonly IMongoQuery _query;

        public EntityReplaceUpdateCommand(MongoCollection<TEntity> collection, TEntity entity)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            _collection = collection;
            _entity = entity;
            _query = Query.And(
                    Query<IEntity>.EQ(p => p.Id, _entity.Id),
                    Query<IEntity>.EQ(p => p.AccessStamp, _entity.AccessStamp)
                );
        }

        public void Execute()
        {
            _entity.RenewAccessStamp();
            IMongoUpdate update = Update<IEntity>.Replace(_entity);
            WriteConcernResult result = _collection.Update(_query, update);
            if (result.DocumentsAffected == 0)
            {
                throw new MongoConcurrencyException(
                    "Entity modified by other writer since being retreived from db Id: " +
                    _entity.Id + ", CLR Type: " + typeof(TEntity).FullName);
            }
        }
    }

    [Serializable]
    public class MongoConcurrencyException : MongoException
    {
        public MongoConcurrencyException(string message)
            : base(message)
        {
        }

        public MongoConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public MongoConcurrencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    public interface IEntity
    {
        string Id { get; }
        string AccessStamp { get; }

        void RenewAccessStamp();
    }

    public class Person : IEntity
    {
        public Person()
        {
            AccessStamp = Guid.NewGuid().ToString("N");
        }

        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        [BsonDateTimeOptions(Representation = BsonType.Document, Kind = DateTimeKind.Utc)]
        public DateTime CreatedOn { get; set; }
        public string AccessStamp { get; private set; }
        public void RenewAccessStamp()
        {
            AccessStamp = Guid.NewGuid().ToString("N");
        }
    }
}