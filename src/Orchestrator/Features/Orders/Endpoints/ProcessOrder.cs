using FastEndpoints;
using FluentValidation;
using Orchestrator.Features.Orders.Domain;

namespace Orchestrator.Features.Orders.Endpoints;

internal static class ProcessOrder
{
    internal record Request(
        string OrderId,
        string ProductId,
        int Quantity
    );

    internal record ResponseError(string Message);

    internal record Response(string Id);

    internal sealed class Validator : Validator<Request>
    {
        public Validator()
        {
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
                req.ProductId,
                req.Quantity);

            var orderResponse = await orderService.CreateAsync(orderRequest, ct);

            if (!orderResponse.IsSuccessResponse)
            {
                var response = new ResponseError("Order creation failed");

                await SendAsync(response, 400, ct);
            }

            var inventoryRequest = new InventoryService.Request(req.OrderId, req.ProductId, req.Quantity);
            var inventoryResponse = await inventoryService.ReserveAsync(inventoryRequest, ct);

            if (!inventoryResponse.IsSuccessResponse)
            {
                await orderService.CancelAsync(orderResponse.Obj!.Id, ct);

                var response = new ResponseError("Inventory reservation failed");

                await SendAsync(response, 400, ct);
            }

            var paymentRequest = new PaymentService.Request(orderResponse.Obj!.Id, req.Quantity * 10);
            var paymentResponse = await paymentService.ProcessAsync(paymentRequest, ct);

            if (!paymentResponse.IsSuccessResponse)
            {
                await inventoryService.ReleaseAsync(inventoryRequest, ct);
                await orderService.CancelAsync(orderResponse.Obj.Id, ct);

                var response = new ResponseError("Payment processing failed");

                await SendAsync(response, 400, ct);
            }

            var endpointResponse = new Response(orderResponse.Obj.Id);

            await SendOkAsync(endpointResponse, ct);
        }
    }
}