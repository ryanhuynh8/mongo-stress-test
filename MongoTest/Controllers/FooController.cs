using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoTest.Models;

namespace MongoTest.Controllers
{
    public class FooController : ApiController
    {
        // GET: api/Foo
        public async Task<IEnumerable<string>> Get()
        {
            /* insert relation data for records */
            List<string> response = new List<string>();
            var connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase("aswigdev");
            var collection = db.GetCollection<BusinessObjectModel>("BusinessObject");
            BsonDocumentFilterDefinition<BusinessObjectModel> filter = new BsonDocumentFilterDefinition<BusinessObjectModel>(new BsonDocument());
            var result = await collection.Find(filter).ToListAsync();
            Random r = new Random();
            foreach (BusinessObjectModel record in result)
            {
                record.Refs = new List<ObjectId>();
                for (int i = 0; i <= 5; i++)
                {
                    int n = r.Next(0, 50000);
                    ObjectId objectId = result[n].Id;
                    record.Refs.Add(objectId);
                    var resultFilter = Builders<BusinessObjectModel>.Filter.Eq(o => o.Id, record.Id);
                    var resultUpdate = Builders<BusinessObjectModel>.Update.Set(o => o.Refs, record.Refs);
                    await collection.UpdateOneAsync(resultFilter, resultUpdate);
                }
            }
            return new string[] { "Ok" };

        }

        // GET: api/Foo/5
        public async Task<IEnumerable<string>> Get(string id)
        {
            /* database connecting */
            string cmd = id;
            var connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase("aswigdev");
            var collection = db.GetCollection<BusinessObjectModel>("BusinessObject");
            var response = new List<string>();

            Random r = new Random();

            /* get all record and random select one */
            //var allRecord = await collection.Find(new BsonDocumentFilterDefinition<BusinessObjectModel>(new BsonDocument())).ToListAsync();
            int n = r.Next(0, 50001);
            response.Add(n.ToString());

            var filter = Builders<BusinessObjectModel>.Filter.Eq(o => o.Id, allRecord[n].Id);
            response.Add(allRecord[n].Id.ToString());
            Stopwatch sw = Stopwatch.StartNew();
            var result = await collection.Find(filter).ToListAsync();
            foreach (ObjectId record in result[0].Refs)
            {
                var refNodeFilter = Builders<BusinessObjectModel>.Filter.Eq(o => o.Id, record);
                var refNode = await collection.Find(refNodeFilter).ToListAsync();
                response.Add(refNode[0].Id.ToString() + " " + refNode[0].Type + " ");
            }
            sw.Stop();
            response.Add(sw.ElapsedMilliseconds.ToString());

            //for (int i = 0; i < 50000; i++)
            //{
            //    var entity = new BusinessObjectModel();
            //    var n = r.Next(1, 4);
            //    string type = "";
            //    if (n == 1) type = "Payment";
            //    if (n == 2) type = "Document";
            //    if (n == 3) type = "Claim";
            //    entity.Type = type;                
            //    await collection.InsertOneAsync(entity);
            //}
            return response;
        }

        // POST: api/Foo
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Foo/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Foo/5
        public void Delete(int id)
        {
        }
    }
}
