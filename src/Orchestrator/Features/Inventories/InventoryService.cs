using Microsoft.AspNetCore.Mvc;

namespace Orchestrator.Features.Inventories;

public static class InventoryService
{
    public record Response();
    
    public record Request(string ProductId, int Quantity);
    
    public interface IService
    {
        [HttpPost("inventory/reserve")]
        public Task<Response?> ReserveAsync(Request req, CancellationToken ct);
        
        [HttpPost("inventory/release")]
        public Task<Response?> ReleaseAsync(Request req, CancellationToken ct);
    }
}