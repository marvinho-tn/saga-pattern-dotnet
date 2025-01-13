using FastEndpoints;
using Orchestrator;
using Orchestrator.Features.Orders.Domain;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();

var apisConfig = builder.Configuration.GetSection("Apis").Get<ApisConfig>();

builder.Services
    .AddRefitClient<OrderService.IService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apisConfig?.Order.Url ?? string.Empty));

builder.Services
    .AddRefitClient<InventoryService.IService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apisConfig?.Inventory.Url ?? string.Empty));

builder.Services
    .AddRefitClient<PaymentService.IService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apisConfig?.Payment.Url ?? string.Empty));

var app = builder.Build();

app.UseFastEndpoints();

app.Run();
