using Refit;
using Worker.Http;

namespace Worker.Domain;

internal static class InventoryService
{
    internal record Request(string OrderId, string ProductId, int Quantity);
    
    internal interface Service
    {
        [Post("/inventory/reserve")]
        public Task<BaseResponse> ReserveAsync(Request req, CancellationToken ct);
        
        [Post("/inventory/release")]
        public Task<BaseResponse> ReleaseAsync(Request req, CancellationToken ct);
    }
}