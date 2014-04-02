using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBMockSample.Infrastructure {

    public interface IEntityRepository<TEntity> where TEntity : BaseEntity {

        MongoCursor<TEntity> GetAll();

        TEntity GetSingle(string id);

        WriteConcernResult Add(TEntity entity);
        WriteConcernResult Delete(TEntity entity);
        WriteConcernResult Update(IMongoQuery mongoQuery, IMongoUpdate mongoUpdate);
    }
}