using Confluent.Kafka;
using FastEndpoints;
using Orchestrator.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();

builder.Services.AddSingleton(builder.Configuration.GetRequiredSection("Kafka").Get<ProducerConfig>()!);

builder.Services.AddTransient<IEventHandler<OrderRegisteredEventHandler.Event>, OrderRegisteredEventHandler.Handler>();

var app = builder.Build();

app.UseFastEndpoints();

app.Run();
