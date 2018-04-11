using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlannt.IntegrateTest
{
    public class MongoDbTest:IWorker
    {
        public void Run()
        {
            
            Test();
            Console.ReadLine();
        }

        void insertOne()
        {
            var doc = new BsonDocument
            {
                {"name","mongodb"},
                {"type","Database"},
                {"count",1},
                {"info",new BsonDocument
                    {
                        {"x",203},
                        {"y",102}
                    }
                }
            };

            var db = GetDatabase();
            var col = db.GetCollection<BsonDocument>("t1");
            //col.InsertOneAsync(doc);
            col.InsertOne(doc);
        }

        void insertMany()
        {
            var docs = Enumerable.Range(0, 100).Select(x => new BsonDocument("counter", x));
            var db = GetDatabase();
            var col = db.GetCollection<BsonDocument>("t1");
            //col.InsertManyAsync(docs);
            col.InsertMany(docs);
        }

        void countDocs()
        {
            var db = GetDatabase();
            var col = db.GetCollection<BsonDocument>("t1");
            //var count = col.CountAsync(new BsonDocument());
            var count = col.Count(new BsonDocument());
            Console.WriteLine("total docs: {0}", count);
        }

        void findFirst()
        {
            var db = GetDatabase();
            var col = db.GetCollection<BsonDocument>("t1");
            var firstDoc = col.Find(new BsonDocument()).FirstOrDefault();
            Console.WriteLine(firstDoc.ToString());
        }

        void findAll()
        {
            var db = GetDatabase();
            var col = db.GetCollection<BsonDocument>("ScadaData");
            var all = col.Find(new BsonDocument()).ToList();
            all.ForEach(x => Console.WriteLine(x));

            Console.WriteLine();

            col.Find(new BsonDocument()).ForEachAsync(x => Console.WriteLine(x));
        }



                void searchOne(string TagNo)
        {
            var db = GetDatabase();
            var col = db.GetCollection<BsonDocument>("ScadaData");
            var filter = Builders<BsonDocument>.Filter.Eq("TagNo", TagNo);
            var doc = col.Find(filter).FirstOrDefault();
            Console.WriteLine(doc);
        }

        void searchOne()
        {
            var db = GetDatabase();
            var col = db.GetCollection<BsonDocument>("t1");
            var filter = Builders<BsonDocument>.Filter.Eq("counter", 71);
            var doc = col.Find(filter).FirstOrDefault();
            Console.WriteLine(doc);
        }

        void searchMany()
        {
            var db = GetDatabase();
            var col = db.GetCollection<BsonDocument>("t1");
            var filter = Builders<BsonDocument>.Filter.Gt("counter", 50);
            col.Find(filter).ForEachAsync(x=>Console.WriteLine(x));
        }

        void sort()
        {
            var db = GetDatabase();
            var col = db.GetCollection<BsonDocument>("t1");
            var filter = Builders<BsonDocument>.Filter.Exists("counter");
            var sortEx = Builders<BsonDocument>.Sort.Descending("counter");
            col.Find(filter).Sort(sortEx).ForEachAsync(x => Console.WriteLine(x));
        }

        void projection()
        {
            var db = GetDatabase();
            var col = db.GetCollection<BsonDocument>("t1");
            var proj = Builders<BsonDocument>.Projection.Exclude("_id");
            Console.WriteLine(col.Find(new BsonDocument()).Project(proj).First());
        }

        void updateOne()
        {
            var db = GetDatabase();
            var col = db.GetCollection<BsonDocument>("t1");
            var filter = Builders<BsonDocument>.Filter.Eq("counter", 1);
            var updated = Builders<BsonDocument>.Update.Set("counter", 110);

            var result = col.UpdateOne(filter, updated);
            Console.WriteLine("MatchedCount={0},ModifiedCount={1},UpsertedId={2}", result.MatchedCount, result.ModifiedCount, result.UpsertedId);
        }

        void updateMany()
        {
            var db = GetDatabase();
            var col = db.GetCollection<BsonDocument>("t1");
            var filter = Builders<BsonDocument>.Filter.Gt("counter", 20);
            var updated = Builders<BsonDocument>.Update.Set("counter", 100);

            var result = col.UpdateMany(filter, updated);
            Console.WriteLine("MatchedCount={0},ModifiedCount={1},UpsertedId={2}", result.MatchedCount, result.ModifiedCount, result.UpsertedId);
        }

        void delete()
        {
            var db = GetDatabase();
            var col = db.GetCollection<BsonDocument>("t1");
            var filter = Builders<BsonDocument>.Filter.Eq("counter", 100);
            var result = col.DeleteMany(filter);
            Console.WriteLine("DeletedCount={0}", result.DeletedCount);
        }

        void deleteAll()
        {
            var db = GetDatabase();
            var col = db.GetCollection<BsonDocument>("t1");
            var filter = Builders<BsonDocument>.Filter.Exists("counter");
            var result = col.DeleteMany(filter);
            Console.WriteLine("DeletedCount={0}", result.DeletedCount);
        }

        void Test()
        {
            Console.WriteLine("insertOne:");
            Console.ReadLine();
            insertOne();
            Console.WriteLine("insertMany:");
            Console.ReadLine();
            insertMany();
            Console.WriteLine("countDocs:");
            Console.ReadLine();
            countDocs();
            Console.WriteLine("findFirst:");
            Console.ReadLine();
            findFirst();
            Console.WriteLine("findAll:");
            Console.ReadLine();
            findAll();
            Console.WriteLine("searchOne:");
            Console.ReadLine();
            searchOne();
            Console.WriteLine("searchMany:");
            Console.ReadLine();
            searchMany();

            Console.WriteLine("sort:");
            Console.ReadLine();
            sort();

            Console.WriteLine("projection:");
            Console.ReadLine();
            projection();

            Console.WriteLine("updateOne:");
            Console.ReadLine();
            updateOne();

            Console.WriteLine("updateMany:");
            Console.ReadLine();
            updateMany();

            Console.WriteLine("findAll:");
            Console.ReadLine();
            findAll();


            Console.WriteLine("delete:");
            Console.ReadLine();
            delete();

            Console.WriteLine("findAll:");
            Console.ReadLine();
            findAll();

            Console.WriteLine("deleteAll:");
            Console.ReadLine();
            deleteAll();

        }

        


        IMongoDatabase GetDatabase()
        {
            var dbName = System.Configuration.ConfigurationManager.AppSettings["mongodbDB"];
            return GetMongoClient().GetDatabase(dbName);
        }

        MongoClient GetMongoClient()
        {
            var mongoDBString = System.Configuration.ConfigurationManager.AppSettings["mongodbServer"];
            var client = new MongoClient(mongoDBString);
            return client;
        }
    }
}
