using FastEndpoints;
using FluentValidation;
using MongoDB.Driver;

namespace Inventory.Features.Inventory.Endpoints;

internal static class Reseerve
{
    internal record Request(string ProductId, int Quantity);

    internal sealed class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }

    internal sealed class Endpoint(IMongoDatabase database) : Endpoint<Request>
    {
        public override void Configure()
        {
            Post("inventory/reserve");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var collection = database.GetCollection<Domain.Inventory>("Inventories");
            var inventory = (await collection
                    .FindAsync(i => i.ProductId == req.ProductId, cancellationToken: ct))
                .FirstOrDefault();

            if (inventory is null || inventory.Stock < req.Quantity)
            {
                await SendAsync(null, 400, ct);
            }

            inventory!.Stock -= req.Quantity;

            await collection.FindOneAndUpdateAsync(i => i.ProductId == inventory.ProductId,
                Builders<Domain.Inventory>.Update.Set(i => i.Stock, inventory.Stock),
                cancellationToken: ct);

            await SendNoContentAsync(ct);
        }
    }
}