using FastEndpoints;
using FluentValidation;
using Orchestrator.Handlers;

namespace Orchestrator.Endpoints;

internal static class ProcessOrder
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

    internal sealed class Endpoint : Endpoint<Request>
    {
        public override void Configure()
        {
            Post("saga/orders");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var @event = new OrderRegisteredEventHandler.Event(req.ProductId, req.Quantity);

            await PublishAsync(@event, cancellation: ct);
            
            await SendAsync(null, 202, ct);
        }
    }
}