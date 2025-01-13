using Refit;

namespace Orchestrator.Features.Orders.Domain;

public static class InventoryService
{
    public record Response();
    
    public record Request(string ProductId, int Quantity);
    
    public interface IService
    {
        [Post("/inventory/reserve")]
        public Task<Response?> ReserveAsync(Request req, CancellationToken ct);
        
        [Post("/inventory/release")]
        public Task<Response?> ReleaseAsync(Request req, CancellationToken ct);
    }
}