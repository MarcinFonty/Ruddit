using Content_Managment_Service.CommandHandlers;
using EventStore.Client;
using RabbitMQ.Client;
using MediatR;
using Content_Managment_Service.Commands;
using Microsoft.Extensions.DependencyInjection;
using Content_Managment_Service.Logic;
using System.Reflection;
using Polly;
using System.Net.Sockets;
using RabbitMQ.Client.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddTransient<IRequestHandler<CreatePostCommand>, CreatePostCommandHandler>();
builder.Services.AddTransient<IPostLogic, PostLogic>();

builder.Services.AddSingleton<EventStoreClient>(sp =>
{
    var settings = EventStoreClientSettings.Create("esdb://127.0.0.1:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000");

    return new EventStoreClient(settings);
});

builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory()
    {
        HostName = "rmq",
        Port = 5672,
        UserName = "guest",
        Password = "guest",
        DispatchConsumersAsync = true
    };

    var retryPolicy = Policy.Handle<SocketException>()
        .WaitAndRetry(new[]
        {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(3)
        });

    return retryPolicy.Execute(() =>
    {
        while (true)
        {
            try
            {
                return factory.CreateConnection();
            }
            catch (Exception ex) when (ex is SocketException || ex is BrokerUnreachableException)
            {
                Console.WriteLine("RabbitMQ Client is trying to connect...");
            }
        }
    });
});

// Configure Kestrel to listen on specific address
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(7278);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var connection = scope.ServiceProvider.GetRequiredService<IConnection>();
    var channel = connection.CreateModel();
    var exchangeName = "post-created-exchange";
    var exchangeType = ExchangeType.Direct;
    var durable = true;
    var autoDelete = false;

    channel.ExchangeDeclare(exchangeName, exchangeType, durable, autoDelete);
    var queueName = "post-created-queue";
    var exclusive = false;
    var arguments = new Dictionary<string, object>();

    channel.QueueDeclare(queueName, durable, exclusive, autoDelete, arguments);
    channel.QueueBind(queueName, exchangeName, "post.created");
}

app.UseSwagger();
app.UseSwaggerUI();

// Remove this line to disable HTTPS redirection
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
