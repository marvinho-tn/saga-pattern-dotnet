using FastEndpoints;
using MongoDB.Driver;

namespace Payment.Endpoints;

internal static class CancelPayment
{
    internal record Request(string OrderId);
    
    internal sealed class Endpoint(IMongoDatabase database) : Endpoint<Request>
    {
        public override void Configure()
        {
            Delete("payments/{OrderId}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var paymentsCollection = database.GetCollection<Domain.Payment>("Payments");
            
            await paymentsCollection.DeleteOneAsync(p => p.OrderId == req.OrderId, ct);
            
            await SendNoContentAsync(ct);
        }
    }
}