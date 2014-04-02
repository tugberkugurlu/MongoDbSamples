using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBMockSample {

    public static class MongoDbTestHelpers {

        private static readonly MongoServerSettings ServerSettings;
        private static readonly MongoServer Server;
        private static readonly MongoDatabaseSettings DatabaseSettings;
        private static readonly MongoDatabase Database;

        static MongoDbTestHelpers() {

            ServerSettings = new MongoServerSettings { 
                Servers = new List<MongoServerAddress> {
                    new MongoServerAddress("unittest")
                }
            };

            Server = new MongoServer(ServerSettings);
            DatabaseSettings = new MongoDatabaseSettings("databaseName", new MongoCredentials("", ""), GuidRepresentation.Standard, ReadPreference.Primary, WriteConcern.Acknowledged);
            Database = new MongoDatabase(Server, DatabaseSettings);
        }

        public static Mock<MongoCollection<T>> CreateMockCollection<T>() {
            
            var collectionSettings = new MongoCollectionSettings<T>(Database, typeof(T).Name);
            var mongoCollectionMock = new Mock<MongoCollection<T>>(Database, collectionSettings);
            mongoCollectionMock.Setup(collection => collection.Database).Returns(Database);
            mongoCollectionMock.Setup(collection => collection.Settings).Returns(collectionSettings);

            return mongoCollectionMock;
        }

        public static MongoCollection<T> ReturnsCollection<T>(this Mock<MongoCollection<T>> collectionMock, IEnumerable<T> enumerable) {

            var cursorMock = new Mock<MongoCursor<T>>(collectionMock.Object, new Mock<IMongoQuery>().Object, ReadPreference.Primary);
            cursorMock.Setup(cursor => cursor.GetEnumerator()).Returns(() => {
                var enumerator = enumerable.GetEnumerator();
                return enumerator;
            });

            cursorMock.Setup(cursor => cursor.SetSortOrder(It.IsAny<IMongoSortBy>())).Returns(cursorMock.Object);
            cursorMock.Setup(cursor => cursor.SetLimit(It.IsAny<int>())).Returns(cursorMock.Object);
            cursorMock.Setup(cursor => cursor.SetFields(It.IsAny<IMongoFields>())).Returns(cursorMock.Object);
            cursorMock.Setup(cursor => cursor.SetFields(It.IsAny<string[]>())).Returns(cursorMock.Object);
            cursorMock.Setup(cursor => cursor.SetSkip(It.IsAny<int>())).Returns(cursorMock.Object);
            cursorMock.Setup(cursor => cursor.Count()).Returns(enumerable.Count());

            collectionMock.Setup(collection => collection.Find(It.IsAny<IMongoQuery>())).Returns(cursorMock.Object);
            collectionMock.Setup(collection => collection.FindAs<T>(It.IsAny<IMongoQuery>())).Returns(cursorMock.Object);
            collectionMock.Setup(collection => collection.FindAll()).Returns(cursorMock.Object);
            collectionMock.Setup(collection => collection.FindAllAs<T>()).Returns(cursorMock.Object);

            return collectionMock.Object;
        }
    }
}