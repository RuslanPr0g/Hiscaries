using System.Net.Http.Json;

namespace Hiscary.IntegrationTesting.Http;

public static class HttpClientJsonExtensions
{
    public static Task<HttpResponseMessage> PostJsonAsync<TRequest>(
        this HttpClient client,
        string requestUri,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        return client.PostAsJsonAsync(requestUri, request, cancellationToken);
    }
}
