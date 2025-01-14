using FastEndpoints;
using Inventory.Handlers;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();

builder.Services.AddTransient<IMongoDatabase>(sp =>
{
    var connectionString = builder.Configuration.GetSection("Database:ConnectionString").Value;
    var client = new MongoClient(connectionString);
    var databaseName = builder.Configuration.GetSection("Database:Name").Value;

    return client.GetDatabase(databaseName);
});

builder.Services.AddTransient<IEventHandler<ReserveAbortedEventHandler.Event>, ReserveAbortedEventHandler.Handler>();

var app = builder.Build();

app.UseFastEndpoints();

app.Run();
