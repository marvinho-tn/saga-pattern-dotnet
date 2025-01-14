using FastEndpoints;
using FluentValidation;
using MongoDB.Driver;

namespace Payment.Endpoints;

internal static class ProcessPayment
{
    internal record Request(string OrderId, int Amount);
    
    internal sealed class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.OrderId).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }
    
    internal record Response(string Id);
    
    internal sealed class Endpoint(IMongoDatabase database) : Endpoint<Request, Response>
    {
        public override void Configure()
        {
            Post("payments");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var payment = new Domain.Payment
            {
                OrderId = req.OrderId,
                Amount = req.Amount,
                Status = "Processed"
            };

            var paymentsCollection = database.GetCollection<Domain.Payment>("Payments");
            
            await paymentsCollection.InsertOneAsync(payment, cancellationToken: ct);
            
            var response = new Response(payment.Id!);
            
            await SendAsync(response, 201, ct);
        }
    }
}