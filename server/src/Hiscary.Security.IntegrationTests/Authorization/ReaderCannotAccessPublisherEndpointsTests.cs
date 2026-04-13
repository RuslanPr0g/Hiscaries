using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Hiscary.Shared.IntegrationTesting.Security;
using System.Net;
using System.Net.Http.Headers;
using Xunit;

namespace Hiscary.Security.IntegrationTests.Authorization;

public class ReaderCannotAccessPublisherEndpointsTests : IClassFixture<ProtectedEndpointsWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly JwtTokenFactory _jwtFactory;

    public ReaderCannotAccessPublisherEndpointsTests(ProtectedEndpointsWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _jwtFactory = factory.CreateJwtFactory();
    }

    [Property(MaxTest = 1, Arbitrary = new[] { typeof(PublisherOnlyEndpointArbitrary) })]
    public Property ReaderJwt_ToPublisherOnlyEndpoint_Returns403(EndpointDescriptor endpoint)
    {
        var token = _jwtFactory.CreateToken("reader");
        var response = SendAuthenticatedRequest(endpoint, token).GetAwaiter().GetResult();
        var is403 = response.StatusCode == HttpStatusCode.Forbidden;
        return Prop.Label(
            is403,
            $"Expected 403 for {endpoint.Method} {endpoint.Path}, got {(int)response.StatusCode}");
    }

    private async Task<HttpResponseMessage> SendAuthenticatedRequest(EndpointDescriptor endpoint, string jwt)
    {
        var request = new HttpRequestMessage(endpoint.HttpMethod, endpoint.Path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        return await _client.SendAsync(request);
    }
}

public static class PublisherOnlyEndpointArbitrary
{
    public static Arbitrary<EndpointDescriptor> Generate()
    {
        var gen = Gen.Elements(SecurityEndpointCatalog.PublisherOnlyEndpoints);
        return gen.ToArbitrary();
    }
}
