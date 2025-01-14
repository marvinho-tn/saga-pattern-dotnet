using Orchestrator.Http;
using Refit;

namespace Orchestrator.Domain;

internal static class PaymentService
{
    internal record Request(string OrderId, int Amount);
    
    internal interface IService
    {
        [Post("/payments")]
        Task<BaseResponse> ProcessAsync(Request request, CancellationToken ct);

        [Delete("/payments/{orderId}")]
        Task<BaseResponse> CancelAsync(string orderId, CancellationToken ct);
    }
}