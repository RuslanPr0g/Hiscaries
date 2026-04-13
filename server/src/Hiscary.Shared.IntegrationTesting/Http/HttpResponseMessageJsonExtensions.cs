using System.Net.Http.Json;

namespace Hiscary.Shared.IntegrationTesting.Http;

public static class HttpResponseMessageJsonExtensions
{
    public static async Task<T> ReadRequiredJsonAsync<T>(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<T>(cancellationToken);
        return content ?? throw new InvalidOperationException("Expected JSON body but response was empty.");
    }
}
