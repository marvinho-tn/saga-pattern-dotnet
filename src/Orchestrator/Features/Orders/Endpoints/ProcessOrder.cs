using FastEndpoints;
using FluentValidation;
using Orchestrator.Features.Orders.Domain;

namespace Orchestrator.Features.Orders.Endpoints;

internal static class ProcessOrder
{
    internal record Request(
        int Id,
        string ProductId,
        int Quantity
    );

    internal record Response(string? Message, Response.OrderResponse? Order)
    {
        public record OrderResponse(
            int Id,
            string ProductId,
            int Quantity,
            string Status);
    };

    internal sealed class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }

    internal sealed class Endpoint(
        OrderService.IService orderService,
        InventoryService.IService inventoryService,
        PaymentService.IService paymentService) : Endpoint<Request>
    {
        public override void Configure()
        {
            Post("saga/orders");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var orderRequest = new OrderService.Request(
                req.Id,
                req.ProductId,
                req.Quantity,
                "Pending");

            var orderResponse = await orderService.CreateAsync(orderRequest, ct);

            if (orderResponse is null)
            {
                var response = new Response("Order creation failed", null);

                await SendAsync(response, 400, ct);
            }

            var inventoryRequest = new InventoryService.Request(req.ProductId, req.Quantity);
            var inventoryResponse = await inventoryService.ReserveAsync(inventoryRequest, ct);

            if (inventoryResponse is null)
            {
                await orderService.CancelAsync(req.Id, ct);

                var response = new Response("Inventory reservation failed", null);

                await SendAsync(response, 400, ct);
            }

            var paymentRequest = new PaymentService.Request(req.Id, req.Quantity * 10);
            var paymentResponse = await paymentService.ProcessAsync(paymentRequest, ct);

            if (paymentResponse is null)
            {
                await inventoryService.ReleaseAsync(inventoryRequest, ct);
                await orderService.CancelAsync(req.Id, ct);

                var response = new Response("Payment processing failed", null);

                await SendAsync(response, 400, ct);
            }

            var orderEndpointResponse = new Response.OrderResponse(
                req.Id,
                req.ProductId, 
                req.Quantity, 
                "Completed");
            
            var endpointResponse = new Response(null, orderEndpointResponse);
            
            await SendAsync(endpointResponse, 200, ct);
        }
    }
}