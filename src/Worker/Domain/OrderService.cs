using Refit;
using Worker.Http;

namespace Worker.Domain;

internal static class OrderService
{
    internal record Response(string Id);
    
    internal record Request(
        string ProductId,
        int Quantity);

    internal interface Service
    {
        [Post("/orders")]
        Task<BaseResponse<Response>> CreateAsync(Request req, CancellationToken ct);
        
        [Delete("/orders/{id}")]
        Task<BaseResponse> CancelAsync(string id, CancellationToken ct);
    }
}