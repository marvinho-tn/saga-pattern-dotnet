using Orchestrator.Http;
using Refit;

namespace Orchestrator.Features.Orders.Domain;

internal static class InventoryService
{
    internal record Request(string OrderId, string ProductId, int Quantity);
    
    internal interface IService
    {
        [Post("/inventory/reserve")]
        public Task<BaseResponse> ReserveAsync(Request req, CancellationToken ct);
        
        [Post("/inventory/release")]
        public Task<BaseResponse> ReleaseAsync(Request req, CancellationToken ct);
    }
}