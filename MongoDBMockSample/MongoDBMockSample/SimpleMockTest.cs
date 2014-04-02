using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDBMockSample.Infrastructure;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MongoDBMockSample {
    
    public class SimpleMockTest {

        public class Person : BaseEntity {

            public string Name { get; set; }
            public string Surname { get; set; }
            public int Age { get; set; }

            public IEnumerable<Session> Sessions { get; set; }
        }

        public class Session : BaseEntity {

            public string SessionName { get; set; }
            public string SalonName { get; set; }
            public Person Person { get; set; }
        }

        [Fact]
        public void Test() {

            var p1 = new Person {
                Name = "Tugberk",
                Surname = "Ugurlu",
                Age = 25,
                Sessions = new List<Session> { 
                    new Session { Id = ObjectId.GenerateNewId(), SessionName = "Ses1", SalonName = "Salon1" },
                    new Session { Id = ObjectId.GenerateNewId(), SessionName = "Ses2", SalonName = "Salon2" },
                    new Session { Id = ObjectId.GenerateNewId(), SessionName = "Ses3", SalonName = "Salon1" }
                }
            };

            var p2 = new Person {
                Name = "Tugberk2",
                Surname = "Ugurlu2",
                Age = 35,
                Sessions = new List<Session> { 
                    new Session { SessionName = "Ses1", SalonName = "Salon1" },
                    new Session { SessionName = "Ses2", SalonName = "Salon2" },
                    new Session { SessionName = "Ses3", SalonName = "Salon1" }
                }
            };

            var qu = Query.And(Query<Person>.EQ(x => x.Id, ObjectId.GenerateNewId()), Query<Person>.EQ(x => x.Name, "Tugberk"));
            var foo1 = Query.And(Query<Person>.GT(x => x.Age, 20), qu);
            var foo = foo1.ToJson();
            var bar = qu.ToJson<dynamic>();

            var collection = MongoDbTestHelpers.CreateMockCollection<Person>()
                .ReturnsCollection(new List<Person> { p1, p2 });

            var personRepo = new EntityRepository<Person>(collection);
            var people = personRepo.GetAll();

            Assert.Equal<int>((int)people.Count(), 2);
        }
    }
}