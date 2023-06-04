using Common.DataModel;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Feed_Service.Repositories
{
    public class FeedRepository : IFeedRepository
    {
        private readonly IMongoCollection<PostModel> collection;
        private readonly IConfiguration _configuration;

        public FeedRepository()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var mongoClient = new MongoClient(_configuration["MongoDB:ConnectionString"]);
            var mongoDatabase = mongoClient.GetDatabase(_configuration["MongoDB:DatabaseName"]);
            collection = mongoDatabase.GetCollection<PostModel>("Posts"); //TODO: Should have a better way to provide the collection name
        }

        public async Task<List<PostModel>> GetPage(int pageSize, DateTime? latestCreatedDate = null)
        {
            var filter = Builders<PostModel>.Filter.Empty;

            if (latestCreatedDate.HasValue)
            {
                var createdDateFilter = Builders<PostModel>.Filter.Gt("CreatedDate", latestCreatedDate.Value);
                filter = Builders<PostModel>.Filter.And(filter, createdDateFilter);
            }

            var sort = Builders<PostModel>.Sort.Descending("CreatedDate");
            var result = await collection.Find(filter)
                .Sort(sort)
                .Limit(pageSize)
                .ToListAsync();

            return result;
        }
    }
}
