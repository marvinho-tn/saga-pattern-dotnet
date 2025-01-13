using FastEndpoints;
using MongoDB.Driver;

namespace Order.Features.Order.Endpoints;

internal static class CancelOrder
{
    internal record Request(string Id);

    internal sealed class Endpoint(IMongoDatabase database) : Endpoint<Request>
    {
        public override void Configure()
        {
            Delete("orders/{Id}");
            AllowAnonymous();
        }
        
        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var collection = database.GetCollection<Domain.Order>("Orders");
            
            await collection.DeleteOneAsync(o => o.Id == req.Id, ct);
            
            await SendNoContentAsync(ct);
        }
    }
}