using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBTry {

    public class Person {

        public BsonObjectId Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }

        public IEnumerable<Session> Sessions { get; set; }
    }

    public class Session {

        public BsonObjectId Id { get; set; }
        public string SessionName { get; set; }
        public string SalonName { get; set; }
        public Person Person { get; set; }
        public Nullable<DateTime> SessionDate { get; set; }
    }

    class Program {

        static void Main(string[] args) {

            var p1 = new Person {
                Name = "Tugberk",
                Surname = "Ugurlu",
                Age = 25,
                Sessions = new List<Session> { 
                    new Session { Id = ObjectId.GenerateNewId(), SessionName = "Ses1", SalonName = "Salon1" },
                    new Session { SessionName = "Ses2", SalonName = "Salon2" },
                    new Session { SessionName = "Ses3", SalonName = "Salon1" }
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

            var db = new MongoClient().GetServer().GetDatabase("Conference");
            //var result1 = db.GetCollection<Person>("People").Insert(new Person {
            //    Name = "Tugberk",
            //    Surname = "Ugurlu",
            //    Age = 25,
            //    Sessions = new List<Session> { 
            //        new Session { Id = ObjectId.GenerateNewId(), SessionName = "Ses1", SalonName = "Salon1" },
            //        new Session { Id = ObjectId.GenerateNewId(), SessionName = "Ses2", SalonName = "Salon2" },
            //        new Session { Id = ObjectId.GenerateNewId(), SessionName = "Ses3", SalonName = "Salon1" }
            //    }
            //}, WriteConcern.Acknowledged);

            //var result2 = db.GetCollection<Person>("People").Insert(new Person {
            //    Name = "Tugberk2",
            //    Surname = "Ugurlu2",
            //    Age = 35,
            //    Sessions = new List<Session> { 
            //        new Session { Id = ObjectId.GenerateNewId(), SessionName = "Ses1", SalonName = "Salon1" },
            //        new Session { Id = ObjectId.GenerateNewId(), SessionName = "Ses2", SalonName = "Salon2" },
            //        new Session { Id = ObjectId.GenerateNewId(), SessionName = "Ses3", SalonName = "Salon1" }
            //    }
            //}, WriteConcern.Acknowledged);

            // {50c7a1e2953a6f0c645829f3}

            //// http://stackoverflow.com/questions/7256723/mongodb-querying-embedded-collection-with-filtering-ordering
            //var slice = Fields.Slice(name: "Sessions", size: 1);
            //var result = db.GetCollection<Person>("People")
            //               .Find(Query<Person>.EQ(x => x.Id, BsonObjectId.Parse("50c7a1e2953a6f0c645829f3")))
            //               .SetLimit(1)
            //               .SetFields(slice)
            //               .ToJson();

            //var res = db.GetCollection<Person>("People").Insert<Session>(new Session { 
            //    Person = result,
            //    SessionName = "Session2",
            //    SalonName = "Salon11"
            //}, WriteConcern.Acknowledged);

            // http://docs.mongodb.org/manual/applications/update/
            // http://stackoverflow.com/questions/7732663/bsonvalue-and-custom-classes-in-mongodb-c-sharp-driver
            //db.GetCollection<Person>("People").Update(
            //    Query<Person>.EQ(person => person.Id, BsonObjectId.Parse("50cd028f953a6f1ab89aa737")),
            //    Update.Push("Sessions", new Session { 
            //        Id = ObjectId.GenerateNewId(), SessionName = "Ses1", SalonName = "Salon1", SessionDate = DateTime.Now.AddDays(20) }.ToBsonDocument()), WriteConcern.Acknowledged);

            var people = db.GetCollection<Person>("People").FindAllAs<Person>().ToList();

            var people2 = db.GetCollection<Person>("People")
                .AsQueryable().Where(x => x.Sessions.Any(y => y.SessionDate != null));

            var peeps = people2.Select(x => 
                x.Sessions.Where(y => y.SessionDate != null)).ToList();
        }
    }
}