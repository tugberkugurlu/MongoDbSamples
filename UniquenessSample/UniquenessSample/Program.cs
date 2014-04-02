using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace UniquenessSample
{
    class Program
    {
        static void Main(string[] args)
        {
            MongoClient client = new MongoClient("mongodb://localhost");
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase("membership");
            var users = db.GetCollection<User>("users", WriteConcern.Acknowledged);

            User user1 = new User("Tugberk", "tugberk@exmaple.com");
            users.Insert(user1);

            try
            {
                User user2 = new User("Tugberk", "tugberk@exmaple.com");
                users.Insert(user2);
            }
            catch (WriteConcernException ex)
            {
                Console.WriteLine(ex);
            }
        }
    }


    public class User
    {
        public User(string userName, string email)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (email == null) throw new ArgumentNullException("email");

            Id = userName.ToLower(CultureInfo.InvariantCulture);
            UserName = userName;
            Email = email;
        }

        [BsonId]
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}