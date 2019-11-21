using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using NTS.Utils.Mongo.Extensions;
using NTS.Utils.Mongo.Model;
 

namespace NTS.Utils.Mongo
{

    public interface IMongoRepository<T> where T : MongoEntity
    {
        Task<T> Find(string id);
        Task<string> Save(T record);
        Task<bool> Update(T record);
        Task<bool> Upsert(T record);
        Task<bool> Delete(string id);
    }

    public class MongoRepositoryBase<T> : IMongoRepository<T> where T : MongoEntity
    {

        protected readonly IMongoClient _mongoClient;
        protected readonly IMongoDatabase _database;
        protected readonly IMongoCollection<T> _recordCollection;

        public MongoRepositoryBase(IMongoClient mongoClient, string databaseName, string collectionName = "attachments")
        {
            databaseName.ThrowIfNullOrEmpty("databaseName");
            mongoClient.ThrowIfNull("mongoClient");

            _mongoClient = mongoClient;
            _database = _mongoClient.GetDatabase(databaseName);
            if (_database == null)
                throw new ApplicationException("Couldn't connect to database {0} on {1}.".F(databaseName, _mongoClient.Settings.Servers.GetStringValue()));
            _recordCollection = _database.GetCollection<T>(collectionName);
            if (_recordCollection == null)
                throw new ApplicationException("Couldn't connect to collection {0}.{1} on {2}.".F(databaseName, collectionName, _mongoClient.Settings.Servers.GetStringValue()));
        }

        public Task<T> Find(string id)
        {
            id.ThrowIfNullOrEmpty("id");
            return _recordCollection.Find(a => a.Id == id).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> Filter(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> defineFilter, int limit = 500)
        {
            if (defineFilter == null)
                throw new ArgumentNullException("defineFilter");

            var builder = Builders<T>.Filter;
            var filter = defineFilter(builder);
            var r = await _recordCollection.Find(filter).Limit(limit).ToListAsync().ConfigureAwait(false);
            return r;
        }

        public async Task<string> Save(T record)
        {
            record.ThrowIfNull("record");

            record.Id = null;
            await _recordCollection.InsertOneAsync(record).ConfigureAwait(false);
            return record.Id;
        }

        public async Task<bool> Update(T record)
        {
            record.ThrowIfNull("record");
            record.Id.ThrowIfNullOrEmpty("record.Id");

            ReplaceOneResult updateResult = await _recordCollection.ReplaceOneAsync(a => a.Id == record.Id, record).ConfigureAwait(false);
            return updateResult.IsAcknowledged && updateResult.IsModifiedCountAvailable && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> Upsert(T record)
        {
            record.ThrowIfNull("record");
            ReplaceOneResult updateResult = await _recordCollection.ReplaceOneAsync(a => a.Id == record.Id, record, new UpdateOptions { IsUpsert = true }).ConfigureAwait(false);
            return updateResult.IsAcknowledged && ((updateResult.IsModifiedCountAvailable && updateResult.ModifiedCount > 0) || updateResult.UpsertedId != null);
        }

        public async Task<bool> Delete(string id)
        {
            id.ThrowIfNullOrEmpty("id");
            DeleteResult deleteResult = await _recordCollection.DeleteOneAsync(a => a.Id == id);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }
    }

} // Namespace 
