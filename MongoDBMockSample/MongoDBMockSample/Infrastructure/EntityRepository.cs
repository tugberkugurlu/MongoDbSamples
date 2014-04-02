using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBMockSample.Infrastructure {

    public class EntityRepository<TEntity> : IEntityRepository<TEntity> where TEntity : BaseEntity {

        private readonly MongoDatabase _mongoDatabase;
        private readonly string _collectionName;
        private readonly MongoCollection<TEntity> _collection;

        public EntityRepository(MongoDatabase mongoDatabase) : this(mongoDatabase, typeof(TEntity).Name) { }

        public EntityRepository(MongoDatabase mongoDatabase, string collectionName) {

            _mongoDatabase = mongoDatabase;
            _collectionName = collectionName;
        }

        internal EntityRepository(MongoCollection<TEntity> collection) {

            _collection = collection;
        }

        public MongoCursor<TEntity> GetAll() {

            var col = GetMongoCollection();
            var res = col.FindAllAs<TEntity>();
            return res;
        }

        public TEntity GetSingle(string id) {

            return GetMongoCollection().FindOneAs<TEntity>(Query<TEntity>.EQ(x => x.Id, BsonValue.Create(id)));
        }

        public WriteConcernResult Add(TEntity entity) {

            return GetMongoCollection().Insert(entity, WriteConcern.Acknowledged);
        }

        public WriteConcernResult Delete(TEntity entity) {

            return GetMongoCollection().Update(
                Query<TEntity>.EQ(x => x.Id, entity.Id), 
                Update<TEntity>.Set(x => x.DeletedOn, DateTime.Now));
        }

        public WriteConcernResult Update(IMongoQuery mongoQuery, IMongoUpdate mongoUpdate) {

            return GetMongoCollection().Update(mongoQuery, mongoUpdate);
        }

        private MongoCollection<TEntity> GetMongoCollection() {

            return (_collection != null) ? _collection : _mongoDatabase.GetCollection<TEntity>(this._collectionName);
        }
    }
}