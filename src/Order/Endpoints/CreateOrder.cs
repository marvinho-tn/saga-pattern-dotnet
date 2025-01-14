using FastEndpoints;
using FluentValidation;
using MongoDB.Driver;
using Order.Handlers;

namespace Order.Endpoints;

internal static class CreateOrder
{
    internal record Request(
        string ProductId,
        int Quantity
    );
    
    internal sealed class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }

    internal record Response(string Id);
    
    internal sealed class Endpoint(IMongoDatabase database) : Endpoint<Request, Response>
    {
        public override void Configure()
        {
            Post("orders");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var ordersCollection = database.GetCollection<Domain.Order>("Orders");
            var order = new Domain.Order
            {
                ProductId = req.ProductId,
                Quantity = req.Quantity,
                Status = "Created"
            };
            
            await ordersCollection.InsertOneAsync(order, cancellationToken: ct);

            var @event = new OrderCreatedEventHandler.Event(order.Id!, order.ProductId, order.Quantity);
            
            await PublishAsync(@event, cancellation: ct);
            
            var response = new Response(order.Id!);
            
            await SendAsync(response, 201, ct);
        }
    }
}