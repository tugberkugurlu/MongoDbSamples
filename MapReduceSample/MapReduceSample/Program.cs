using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MapReduceSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // http://wiki.summercode.com/mongodb_aggregation_functions_and_ruby_map_reduce_basics
            // http://odetocode.com/blogs/scott/archive/2012/03/19/a-simple-mapreduce-with-mongodb-and-c.aspx

            MongoClient client = new MongoClient("mongodb://localhost");
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase("mongodemo");
            MongoContext context = new MongoContext(db);

            // Init(context);

            const string map = @"
                function() {
                    emit(this.AuthorName, { votes: this.VotesCount, count: 1 });
                }";

            const string reduce = @"
                function(key, values) {
                    var sum = 0,
				                count = 0;
				
                    values.forEach(function(doc) {
                        sum += doc.votes;
				                count += 1;
                    });
                    return { votes: sum, count: count };
                }";

            MapReduceArgs options = new MapReduceArgs
            {
                MapFunction = map,
                ReduceFunction = reduce,
                OutputCollectionName = "userVotes",
                OutputMode = MapReduceOutputMode.Replace
            };

            context.Comments.MapReduce(options);

            List<MapReduceResult<UserVote>> votes = context.UserVotes.FindAll().ToList();
            foreach (MapReduceResult<UserVote> userVote in votes)
            {
                Console.WriteLine("{0}, {1}, {2}", userVote.Id, userVote.Value.Votes, userVote.Value.Count);
            }

            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        static void Init(MongoContext context)
        {
            IList<Comment> comments = new List<Comment>();
            for (int i = 0; i < 5000; i++)
            {
                comments.Add(new Comment
                {
                    Content = Faker.Lorem.Sentence(10),
                    AuthorName = Faker.Name.First(),
                    VotesCount = Faker.RandomNumber.Next(0, 225)
                });
            }

            context.Comments.InsertBatch(comments, WriteConcern.Acknowledged);
        }
    }

    public class MongoContext
    {
        public MongoContext(MongoDatabase database)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }

            Comments = database.GetCollection<Comment>("comments");
            UserVotes = database.GetCollection<MapReduceResult<UserVote>>("userVotes");
        }

        public MongoCollection<Comment> Comments { get; private set; }
        public MongoCollection<MapReduceResult<UserVote>> UserVotes { get; private set; }
    }

    public class MapReduceResult<T>
    {
        public string Id { get; set; }

        [BsonElement("value")]
        public T Value { get; set; }
    }

    public class UserVote
    {
        [BsonElement("votes")]
        public int Votes { get; set; }

        [BsonElement("count")]
        public int Count { get; set; }
    }

    public class Comment
    {
        public string Content { get; set; }
        public string AuthorName { get; set; }
        public int VotesCount { get; set; }
    }
}