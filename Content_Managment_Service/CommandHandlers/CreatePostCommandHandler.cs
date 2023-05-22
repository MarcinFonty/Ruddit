using Common.Events;
using Content_Managment_Service.Commands;
using EventStore.Client;
using MediatR;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Content_Managment_Service.CommandHandlers
{

    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand>
    {
        private readonly IConnection _connection;
        private readonly EventStoreClient _eventStoreClient;
        public CreatePostCommandHandler(IConnection connection, EventStoreClient client)
        {
            _connection = connection;
            _eventStoreClient = client;
        }
        public async Task Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            var @event = new PostCreateEvent
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                ImagePath = request.ImagePath,
                Author = request.Author,
                CreatedDate = request.CreatedDate
            };

            await AppendEvent(@event);

            await PublishEvent(@event);
        }

        private async Task AppendEvent(PostCreateEvent @event)
        {
            var eventData = new EventData(
                Uuid.NewUuid(),
                @event.GetType().Name,
                JsonSerializer.SerializeToUtf8Bytes(@event)
            );
            await _eventStoreClient.AppendToStreamAsync(
                "post-stream", //TODO: Make sure this is correct
                StreamState.Any,
                new[] { eventData }
                //cancellationToken: cancellationToken
            );
        }

        private async Task<bool> PublishEvent(PostCreateEvent @event)
        {
            var channel = _connection.CreateModel();
            var exchangeName = "post-created-exchange"; //TODO: should be a provided variable
            var routingKey = "post.created";
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
            channel.BasicPublish(exchangeName, routingKey, null, body); //TODO: should be changed to be async

            return true;
        }
    }
}
