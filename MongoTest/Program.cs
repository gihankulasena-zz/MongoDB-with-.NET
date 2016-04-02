using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoTest
{
    class Program
    {

        static void Main(string[] args)
        {         
            //meetups mtupInsert = new meetups();
            //mtupInsert.name = "Last1";
            //mtupInsert.speaker = "Last1";

            //meetups mtupUpdate = new meetups();
            //mtupUpdate.Id = new ObjectId("56b35041139d2fb13097eda3");

            //meetups mtupDelete = new meetups();
            //mtupDelete.Id = new ObjectId("57000b3fbbfde41adcbc613d");          

            CRUD.initialize();
            //CRUD.GetMeetup(mtupUpdate.Id);
            //CRUD.InsertMeetup(mtupInsert).Wait();
            //CRUD.DeleteMeetup(mtupDelete).Wait();
            //CRUD.UpdateMeetup(mtupUpdate).Wait();
            CRUD.GetAllMeetups().Wait();
        }
    }


    public class CRUD
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;

        //set client and database name in intialize()
        public static void initialize()
        {
            try
            {
                _client = new MongoClient();
                _database = _client.GetDatabase("mean-demo");
            }
            catch (Exception)
            {                
                throw;
            }       
        
        }

        public static async Task GetAllMeetups()
        {
            try
            {
                //initialize();
                var col = _database.GetCollection<meetups>("meetups");

                var list = await col.Find(new BsonDocument())
                    .ToListAsync();

                foreach (var doc in list)
                {
                    Console.WriteLine("ID : " + doc.Id);
                    Console.WriteLine("Name : " + doc.name);
                    Console.WriteLine("Speaker : " + doc.speaker);
                    Console.WriteLine("Last Modified : " + doc.lastModified);
                    Console.WriteLine("------------------------------------");
                }

                Console.ReadLine();
            }
            catch (Exception)
            {                
                throw;
            }


        }
        public static void GetMeetup(ObjectId id)
        {
            try
            {
                var col = _database.GetCollection<meetups>("meetups");
                var _meetup = col.Find(x => x.Id == id).ToListAsync().Result;

                foreach (var meetup in _meetup)
                {
                    Console.WriteLine("Meetup Name : " + meetup.name);
                    Console.WriteLine("Meetup Speaker : " + meetup.speaker);
                }

                Console.ReadLine();
            }
            catch (Exception)
            {
                throw;
            }


        }

        public static async Task InsertMeetup(meetups meetup)
        {
            try
            {
                //initialize();
                _client = new MongoClient();
                _database = _client.GetDatabase("mean-demo");
                var col = _database.GetCollection<meetups>("meetups");
                await col.InsertOneAsync(meetup);
            }
            catch (Exception)
            {                
                throw;
            }
        }

        public static async Task UpdateMeetup(meetups meetupUpdate)
        {
            try
            {
                //initialize();
                string originalName = null;
                string originalSpeaker = null;

                var col = _database.GetCollection<BsonDocument>("meetups");

                //getting original meetup record
                var original = _database.GetCollection<meetups>("meetups");
                var _meetup = original.Find(x => x.Id == meetupUpdate.Id).ToListAsync().Result;
               
                foreach (var meetup in _meetup)
                {
                    originalName = meetup.name;
                    originalSpeaker = meetup.speaker;
                }
                //end of getting original meetup record

                var filter = Builders<BsonDocument>.Filter.Eq("_id", meetupUpdate.Id);

                var update = Builders<BsonDocument>.Update
                    .Set("name", originalName + " edited")
                    .Set("speaker", originalSpeaker + " edited")
                    .CurrentDate("lastModified");

                await col.UpdateOneAsync(filter, update);
            }
            catch (Exception)
            {                
                throw;
            }
        }

        public static async Task DeleteMeetup(meetups meetupDelete)
        {
            try
            {
                //initialize();
                var col = _database.GetCollection<BsonDocument>("meetups");

                var filter = Builders<BsonDocument>.Filter.Eq("_id", meetupDelete.Id);

                await col.DeleteManyAsync(filter);
            }
            catch (Exception)
            {                
                throw;
            }
        }
    }
    
    public class meetups
    {
        //map real mongodb to here
        public ObjectId Id { get; set; }
        public string name { get; set; }
        public string speaker { get; set; }
        public DateTime lastModified { get; set; }
    }

}
