using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using NTS.Utils.Mongo.Extensions;

namespace NTS.Utils.Mongo
{
    public class MongoRepositoryAttachment<T> : IMongoRepository<T> where T : Mongo.Model.Attachment
    {

        protected readonly IMongoClient _mongoClient;
        protected readonly IMongoDatabase _database;
        protected readonly IMongoCollection<T> _recordCollection;

        public MongoRepositoryAttachment(IMongoClient mongoClient, string databaseName, string collectionName = "attachments")
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


        public Task<T> Find(string id,string propertyId , string parentId , string parentType , string title,string createdById)
        {
            id.ThrowIfNullOrEmpty("id");
            return _recordCollection.Find(a => a.Id == id && a.ParentId == parentId && a.ParentType == parentType &&
                                          a.Title == title  && a.CreatedById == createdById).SingleOrDefaultAsync();
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

        public async Task<string> UpsertAttachment(T record)
        {
            record.ThrowIfNull("record");
            ReplaceOneResult updateResult = await _recordCollection.ReplaceOneAsync(a =>  
                                                      a.ParentId == record.ParentId && 
                                                     a.ParentType == record.ParentType && a.Title == record.Title && a.CreatedOn==record.CreatedOn
                                                     && a.CreatedById ==record.CreatedById, record, 
                                                     new UpdateOptions { IsUpsert = true }).ConfigureAwait(false);
            //return updateResult.IsAcknowledged && ((updateResult.IsModifiedCountAvailable 
            //                                   && updateResult.ModifiedCount > 0) || updateResult.UpsertedId != null);

            if (updateResult.IsModifiedCountAvailable && updateResult.ModifiedCount > 0)
            {
                return record.Id;
            }
            return updateResult.UpsertedId.ToString();


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
}
