using Microsoft.AspNetCore.Mvc;

namespace Orchestrator.Features.Payments;

public static class PaymentService
{
    public record Response();
    
    public record Request(int OrderId, int Amount);
    
    public interface IService
    {
        [HttpPost("payments")]
        Task<Response?> ProcessAsync(Request request, CancellationToken ct);
    }
}