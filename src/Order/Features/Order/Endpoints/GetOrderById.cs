using FastEndpoints;
using MongoDB.Driver;

namespace Order.Features.Order.Endpoints;

internal static class GetOrderById
{
    internal record Request(string Id);
    
    internal record Response(string Id, string ProductId, int Quantity, string Status);

    internal sealed class Endpoint(IMongoDatabase database) : Endpoint<Request, Response>
    {
        public override void Configure()
        {
            Get("orders/{Id}");
            AllowAnonymous();
        }
        
        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var ordersCollection = database.GetCollection<Domain.Order>("Orders");
            var response = (await ordersCollection.FindAsync(o => o.Id == req.Id, cancellationToken: ct))
                .ToList()
                .Select(o => new Response(o.Id!, o.ProductId, o.Quantity, o.Status))
                .FirstOrDefault();
            
            if (response is null)
            {
                await SendNotFoundAsync(ct);
            }
            
            await SendOkAsync(response!, ct);
        }
    }
}