using Confluent.Kafka;
using FastEndpoints;
using Orchestrator;
using Orchestrator.Features.Orders.Consumers;
using Orchestrator.Features.Orders.Domain;
using Orchestrator.Features.Orders.Handlers;
using Orchestrator.Http;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();

var apisConfig = builder.Configuration.GetRequiredSection("Apis").Get<ApisConfig>();

builder.Services.AddCustomRefitClient<OrderService.IService>(apisConfig!.Order.Url);
builder.Services.AddCustomRefitClient<InventoryService.IService>(apisConfig.Inventory.Url);
builder.Services.AddCustomRefitClient<PaymentService.IService>(apisConfig.Payment.Url);

builder.Services.AddSingleton(builder.Configuration.GetRequiredSection("Kafka").Get<ProducerConfig>()!);
builder.Services.AddSingleton(builder.Configuration.GetRequiredSection("Kafka").Get<ConsumerConfig>()!);

builder.Services.AddTransient<IEventHandler<OrderRegisteredEventHandler.Event>, OrderRegisteredEventHandler.Handler>();

builder.Services.AddHostedService<OrderRegisteredConsumer.Consumer>();

var app = builder.Build();

app.UseFastEndpoints();

app.Run();
