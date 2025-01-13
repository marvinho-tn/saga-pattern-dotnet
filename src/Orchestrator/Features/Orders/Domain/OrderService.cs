using Refit;

namespace Orchestrator.Features.Orders.Domain;

public static class OrderService
{
    public record Response();
    
    public record Request(
        int Id,
        string ProductId,
        int Quantity,
        string Status);

    public interface IService
    {
        [Post("/orders")]
        Task<Response?> CreateAsync(Request req, CancellationToken ct);
        
        [Delete("/orders/{id}")]
        Task<Response?> CancelAsync(int id, CancellationToken ct);
    }
}