using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace MongoTest.Models
{
    public class BusinessObjectModel
    {
        public ObjectId Id { get; set; }
        public string Type { get; set; }
        public List<ObjectId> Refs { get; set; }
    }
}