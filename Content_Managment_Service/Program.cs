using Content_Managment_Service.CommandHandlers;
using EventStore.Client;
using RabbitMQ.Client;
using MediatR;
using Content_Managment_Service.Commands;
using Microsoft.Extensions.DependencyInjection;
using Content_Managment_Service.Logic;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
    var factory = new ConnectionFactory() //TODO: Change default credentials and have them as a secret
    {
        HostName = "localhost",
        Port = 5672,
        UserName = "guest",
        Password = "guest",
        DispatchConsumersAsync = true
    };

    return factory.CreateConnection();
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{

    // Declare the exchange
    var connection = scope.ServiceProvider.GetRequiredService<IConnection>();
    var channel = connection.CreateModel();
    var exchangeName = "post-created-exchange";
    var exchangeType = ExchangeType.Direct;
    var durable = true;
    var autoDelete = false;

    channel.ExchangeDeclare(exchangeName, exchangeType, durable, autoDelete);

    // Declare a queue and bind it to the exchange
    var queueName = "post-created-queue";
    var exclusive = false;
    var arguments = new Dictionary<string, object>();

    channel.QueueDeclare(queueName, durable, exclusive, autoDelete, arguments);
    channel.QueueBind(queueName, exchangeName, "post.created");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
