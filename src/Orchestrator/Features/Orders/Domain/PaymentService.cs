using Orchestrator.Http;
using Refit;

namespace Orchestrator.Features.Orders.Domain;

internal static class PaymentService
{
    internal record Request(string OrderId, int Amount);
    
    internal interface IService
    {
        [Post("/payments")]
        Task<BaseResponse> ProcessAsync(Request request, CancellationToken ct);
    }
}