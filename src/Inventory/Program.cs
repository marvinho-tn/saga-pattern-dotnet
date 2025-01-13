using FastEndpoints;
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

var app = builder.Build();

app.UseFastEndpoints();

app.Run();
