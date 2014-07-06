using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace ConsoleApplication5
{
    class Program
    {
        static void Main(string[] args)
        {
            MongoClient client = new MongoClient();
            MongoServer server = client.GetServer();
            MongoDatabase database = server.GetDatabase("conf");
            MongoCollection<Person> peopleCollection = database.GetCollection<Person>("people");
            peopleCollection.Drop();

            Person person = new Person("Tugberk");
            person.AddSession(new Session("Foo Bar-1"));
            person.AddSession(new Session("Foo Bar-2"));
            person.AddSession(new Session("Foo Bar-3"));

            Person personWithNoSession = new Person("Tugberk-2");

            peopleCollection.Insert(person);
            peopleCollection.Insert(personWithNoSession);

            IMongoQuery query1 = Query<Person>.EQ(p => p.Name, "Tugberk");
            Person person1 = peopleCollection.FindOne(query1);

            IMongoQuery query2 = Query<Person>.EQ(p => p.Name, "Tugberk-2");
            Person person2 = peopleCollection.FindOne(query2);
        }
    }

    public class Person
    {
        private readonly List<Session> _sessions;

        [BsonConstructor]
        private Person()
        {
            _sessions = new List<Session>();
        }

        public Person(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            _sessions = new List<Session>();
            Name = name;
        }

        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; private set; }
        public string Name { get; private set; }

        public IEnumerable<Session> Sessions
        {
            get
            {
                return _sessions;
            }

            private set
            {
                _sessions.AddRange(value);
            }
        }

        public void AddSession(Session session)
        {
            if (session == null) throw new ArgumentNullException("session");

            _sessions.Add(session);
        }

        public void RemoveSession(Session session)
        {
            if (session == null) throw new ArgumentNullException("session");

            _sessions.Remove(session);
        }
    }

    public class Session
    {
        [BsonConstructor]
        private Session()
        {
        }

        public Session(string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            Name = name;
            CreatedOn = DateTime.UtcNow;
        }

        public string Name { get; private set; }
        public DateTime CreatedOn { get; private set; }
    }
}
