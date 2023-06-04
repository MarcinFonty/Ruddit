using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text;
using Common.Events;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.Xml.Linq;
using System.IO;
using System;
using Common.DataModel;

namespace Event_Handler_Service.Consumers
{
    public class PostCreateEventConsumer : AsyncEventingBasicConsumer
    {
        private readonly IConfiguration _configuration;
        public PostCreateEventConsumer(IModel model) : base(model)
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public override async Task HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            var json = Encoding.UTF8.GetString(body.ToArray());
            var postCreateEvent = JsonSerializer.Deserialize<PostCreateEvent>(json);

            PostModel result = new PostModel
            {
                Id = postCreateEvent.Id,
                Title = postCreateEvent.Title,
                Description = postCreateEvent.Description,
                ImagePath = postCreateEvent.ImagePath,
                Author = postCreateEvent.Author,
                CreatedDate = postCreateEvent.CreatedDate,
            };

            // TODO: Handle the received message here
            var mongoClient = new MongoClient(_configuration["MongoDB:ConnectionString"]);
            var mongoDatabase = mongoClient.GetDatabase(_configuration["MongoDB:DatabaseName"]);

            var collection = mongoDatabase.GetCollection<PostModel>("Posts");
            collection.InsertOne(result);

            Model.BasicAck(deliveryTag, false);
        }
    }
}
