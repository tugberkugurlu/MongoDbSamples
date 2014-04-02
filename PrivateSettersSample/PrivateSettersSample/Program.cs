using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace PrivateSettersSample
{
    class Program
    {
        static void Main(string[] args)
        {
            MongoClient client = new MongoClient("mongodb://localhost");
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase("mongodemo");
            MongoContext context = new MongoContext(db);

            // Write(context.People);
            // Query(context.People);
        }

        static void Write(MongoCollection<Person> peopleSet)
        {
            Person person1 = new Person("Tugberk", "Ugurlu", 27);
            person1.AddSession(new Session("Ses1", "Salon1"));
            person1.AddSession(new Session("Ses2", "Salon1"));

            Person person2 = new Person("Tugberk2", "Ugurlu2", 29);
            person2.AddSession(new Session("Ses32323", "Salon3"));
            person2.AddSession(new Session("Ses17", "Salon1"));

            peopleSet.InsertBatch(new[] { person1, person2 }, WriteConcern.Acknowledged);
        }

        static void Query(MongoCollection<Person> peopleSet)
        {
            List<Person> people = peopleSet.AsQueryable().ToList();
        }
    }

    public class MongoContext
    {
        public MongoContext(MongoDatabase database)
        {
            if (database == null) throw new ArgumentNullException("database");

            People = database.GetCollection<Person>("people");
        }

        public MongoCollection<Person> People { get; private set; }
    }

    public class Person
    {
        private List<Session> _sessions;

        public Person(string name, string surname, byte age)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (surname == null) throw new ArgumentNullException("surname");

            _sessions = new List<Session>();
            Id = ObjectId.GenerateNewId();
            Name = name;
            Surname = surname;
            Age = age;
        }

        public ObjectId Id { get; private set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public byte Age { get; private set; }

        public IEnumerable<Session> Sessions
        {
            get 
            { 
                return _sessions; 
            }

            private set 
            {
                if (_sessions == null)
                {
                    _sessions = new List<Session>();
                }

                _sessions.AddRange(value); 
            }
        }

        public void AddSession(Session session)
        {
            if (session == null) throw new ArgumentNullException("session");
            _sessions.Add(session);
        }
    }

    public class Session
    {
        public Session(string name, string salonName) 
            : this(name, salonName, null)
        {
        }

        public Session(string name, string salonName, DateTimeOffset? sessionDate)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (salonName == null) throw new ArgumentNullException("salonName");

            Id = Guid.NewGuid().ToString("N");
            Name = name;
            SalonName = salonName;
            SessionDate = sessionDate;
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public string SalonName { get; private set; }
        public DateTimeOffset? SessionDate { get; set; }
    }
}