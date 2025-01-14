using System.Text.Json;
using Refit;

namespace Orchestrator.Http;

public class BaseResponse
{
    public required bool IsSuccessResponse { get; set; }
    public object? Obj { get; set; }
}

public class BaseResponse<T> : BaseResponse
{
    public new T? Obj { get; set; }
}

public class CustomExceptionHandler : DelegatingHandler
{
    public CustomExceptionHandler(HttpMessageHandler innerHandler)
    {
        InnerHandler = innerHandler;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
        var content = new BaseResponse
        {
            IsSuccessResponse = response.IsSuccessStatusCode,
            Obj = !string.IsNullOrEmpty(responseString) ? JsonSerializer.Deserialize<object>(responseString) : null
        };

        response.EnsureSuccessStatusCode();

        response.Content = new StringContent(JsonSerializer.Serialize(
            content,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        ));

        return response;
    }
}

public static class HttpExtensions
{
    public static void AddCustomRefitClient<T>(this IServiceCollection services, string baseUrl) where T : class
    {
        services
            .AddRefitClient<T>(sp =>
            {
                var handler = new HttpClientHandler();

                return new RefitSettings
                {
                    HttpMessageHandlerFactory = () => new CustomExceptionHandler(handler)
                };
            })
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));
    }
}