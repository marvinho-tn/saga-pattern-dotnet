using Confluent.Kafka;
using FastEndpoints;
using MongoDB.Driver;
using Order.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();

builder.Services.AddTransient<IMongoDatabase>(sp =>
{
    var connectionString = builder.Configuration.GetSection("Database:ConnectionString").Value;
    var client = new MongoClient(connectionString);
    var databaseName = builder.Configuration.GetSection("Database:Name").Value;

    return client.GetDatabase(databaseName);
});

builder.Services.AddSingleton(builder.Configuration.GetRequiredSection("Kafka").Get<ProducerConfig>()!);

builder.Services.AddTransient<IEventHandler<OrderCreatedEventHandler.Event>, OrderCreatedEventHandler.Handler>();

var app = builder.Build();

app.UseFastEndpoints();

app.Run();
