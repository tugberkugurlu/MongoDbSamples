using System.Collections.Concurrent;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace FindAndModifySample
{
    class Program
    {
        static void Main(string[] args)
        {
            ConcurrentBag<KeyValuePair<string, string>> questionIdUserNamePairs = new ConcurrentBag<KeyValuePair<string, string>>();
            MongoClient client = new MongoClient("mongodb://localhost");
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase("MongoFMDemo");
            MongoContext context = new MongoContext(db);

            Init(context.Questions);

            IEnumerable<Question> questions = context.Questions.FindAll().ToList();
            foreach (Question question in questions)
            {
                Parallel.For(0, 1000, i =>
                {
                    string userName = Faker.Name.First();
                    IMongoQuery query = Query.And(
                            Query<Question>.EQ(q => q.Id, question.Id),
                            Query<Question>.EQ(q => q.AssignmentRecord, null)
                        );

                    UpdateBuilder<Question> update = Update<Question>.Set(q => q.AssignmentRecord, new AssignmentRecord
                    {
                        User = new User { Name = userName },
                        AssignedOn = DateTimeOffset.UtcNow
                    });

                    FindAndModifyArgs fmArgs = new FindAndModifyArgs
                    {
                        Query = query,
                        Update = update,
                        VersionReturned = FindAndModifyDocumentVersion.Modified,
                        Fields = Fields<Question>.Exclude(q => q.Id).Include(q => q.AssignmentRecord.User.Name)
                    };

                    FindAndModifyResult result = context.Questions.FindAndModify(fmArgs);
                    if (result.ModifiedDocument != null)
                    {
                        // string modifiedUserName = result.ModifiedDocument["AssignmentRecord.User.Name"].AsString;
                        ModifiedAssignmentRecord modifiedDoc = result.GetModifiedDocumentAs<ModifiedAssignmentRecord>();
                        KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(question.Id, modifiedDoc.AssignmentRecord.User.Name);

                        Console.WriteLine(kvp);
                        questionIdUserNamePairs.Add(kvp);
                    }
                });
            }

            IEnumerable<string> keys = questionIdUserNamePairs.Select(kvp => kvp.Key);
            List<IGrouping<string, string>> duplicates = keys.GroupBy(key => key).Where(g => g.Count() > 1).ToList();
            if (duplicates.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed! There are duplicates:");
                Console.WriteLine(Environment.NewLine);
                foreach (IGrouping<string, string> duplicate in duplicates)
                {
                    Console.WriteLine(duplicate.Key);
                }
            }

            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        static void Init(MongoCollection<Question> collection)
        {
            collection.InsertBatch(Enumerable.Range(0, 1000)
                .Select(i => new Question {Title = Faker.Lorem.Sentence()}));
        }

        private class ModifiedAssignmentRecord
        {
            public AssignmentRecord AssignmentRecord { get; set; }
        }
    }

    public class MongoContext
    {
        public MongoContext(MongoDatabase database)
        {
            if (database == null) throw new ArgumentNullException("database");

            Questions = database.GetCollection<Question>("questions");
        }

        public MongoCollection<Question> Questions { get; private set; }
    }

    public class Question
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Title { get; set; }
        public AssignmentRecord AssignmentRecord { get; set; }
    }

    public class AssignmentRecord
    {
        public User User { get; set; }
        public DateTimeOffset AssignedOn { get; set; }
    }

    public class User
    {
        public string Name { get; set; }
    }
}