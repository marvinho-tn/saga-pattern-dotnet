using Confluent.Kafka;
using Worker;
using Worker.Consumers;
using Worker.Domain;
using Worker.Http;

var builder = WebApplication.CreateBuilder(args);

var apisConfig = builder.Configuration.GetRequiredSection("Apis").Get<ApisConfig>();

builder.Services.AddSingleton(builder.Configuration.GetRequiredSection("Kafka").Get<ConsumerConfig>()!);

builder.Services.AddCustomRefitClient<OrderService.Service>(apisConfig!.Order.Url);
builder.Services.AddCustomRefitClient<InventoryService.Service>(apisConfig.Inventory.Url);
builder.Services.AddCustomRefitClient<PaymentService.Service>(apisConfig.Payment.Url);

builder.Services.AddHostedService<OrderRegisteredConsumer.Consumer>();
builder.Services.AddHostedService<OrderCreatedConsumer.InventoryConsumer>();
builder.Services.AddHostedService<OrderCreatedConsumer.PaymentConsumer>();
builder.Services.AddHostedService<ReserveAbortedConsumer.InventoryConsumer>();
builder.Services.AddHostedService<ReserveAbortedConsumer.PaymentConsumer>();

var app = builder.Build();

app.Run();