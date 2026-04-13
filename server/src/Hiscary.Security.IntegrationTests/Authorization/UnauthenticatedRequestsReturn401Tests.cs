using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Hiscary.IntegrationTesting.Security;
using System.Net;
using Xunit;

namespace Hiscary.Security.IntegrationTests.Authorization;

public class UnauthenticatedRequestsReturn401Tests : IClassFixture<ProtectedEndpointsWebAppFactory>
{
    private readonly HttpClient _client;

    public UnauthenticatedRequestsReturn401Tests(ProtectedEndpointsWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Property(MaxTest = 1, Arbitrary = new[] { typeof(ProtectedEndpointArbitrary) })]
    public Property UnauthenticatedRequest_ToProtectedEndpoint_Returns401(EndpointDescriptor endpoint)
    {
        var response = SendUnauthenticatedRequest(endpoint).GetAwaiter().GetResult();
        var is401 = response.StatusCode == HttpStatusCode.Unauthorized;
        return Prop.Label(
            is401,
            $"Expected 401 for {endpoint.Method} {endpoint.Path}, got {(int)response.StatusCode}");
    }

    private async Task<HttpResponseMessage> SendUnauthenticatedRequest(EndpointDescriptor endpoint)
    {
        var request = new HttpRequestMessage(endpoint.HttpMethod, endpoint.Path);
        return await _client.SendAsync(request);
    }
}

public static class ProtectedEndpointArbitrary
{
    public static Arbitrary<EndpointDescriptor> Generate()
    {
        var gen = Gen.Elements(SecurityEndpointCatalog.ProtectedEndpoints);
        return gen.ToArbitrary();
    }
}
