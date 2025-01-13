using Refit;

namespace Orchestrator.Features.Orders.Domain;

public static class OrderService
{
    public record Response(string Id);
    
    public record Request(
        string ProductId,
        int Quantity);

    public interface IService
    {
        [Post("/orders")]
        Task<Response?> CreateAsync(Request req, CancellationToken ct);
        
        [Delete("/orders/{id}")]
        Task<Response?> CancelAsync(string id, CancellationToken ct);
    }
}