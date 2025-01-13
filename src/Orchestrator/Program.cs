using FastEndpoints;
using Orchestrator;
using Orchestrator.Features.Orders.Domain;
using Orchestrator.Http;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();

var apisConfig = builder.Configuration.GetRequiredSection("Apis").Get<ApisConfig>();

builder.Services.AddCustomRefitClient<OrderService.IService>(apisConfig!.Order.Url);
builder.Services.AddCustomRefitClient<InventoryService.IService>(apisConfig.Inventory.Url);
builder.Services.AddCustomRefitClient<PaymentService.IService>(apisConfig.Payment.Url);

var app = builder.Build();

app.UseFastEndpoints();

app.Run();
