using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace NTS.Utils.Mongo.Model
{
    public class MongoEntity
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfNull]
        public string Id { get; set; }
    }

    public class MongoConnection
    {
       public IMongoClient MClient { get; set; }

        public MongoConnection(string mongoDbConnection)
        {
            MClient = new MongoClient(mongoDbConnection);
        }



    }




}
