using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Hiscary.Shared.Domain.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Hiscary.Security.Tests.Authorization;

/// <summary>
/// Property-Based Tests for reader role authorization enforcement.
///
/// Validates: Requirements 2.1, 3.1
///
/// Property: ∀ JWT with `role=reader` sent to `RequirePublisher` endpoints → status 403
/// </summary>
public class ReaderCannotAccessPublisherEndpointsTests : IClassFixture<ProtectedEndpointsWebAppFactory>
{
    private const string TestJwtKey = "test-security-key-for-hiscary-rbac-pbt-tests-32chars";
    private const string TestIssuer = "hiscary-test";
    private const string TestAudience = "hiscary-test";

    private readonly HttpClient _client;

    public ReaderCannotAccessPublisherEndpointsTests(ProtectedEndpointsWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    /// <summary>
    /// Validates: Requirements 2.1, 3.1
    ///
    /// Property: JWT with `role=reader` sent to `RequirePublisher` endpoints → status 403
    /// </summary>
    [Property(MaxTest = 1, Arbitrary = new[] { typeof(PublisherOnlyEndpointArbitrary) })]
    public Property ReaderJwt_ToPublisherOnlyEndpoint_Returns403(PublisherOnlyEndpoint endpoint)
    {
        var token = GenerateReaderJwt();
        var response = SendAuthenticatedRequest(endpoint, token).GetAwaiter().GetResult();
        var is403 = response.StatusCode == HttpStatusCode.Forbidden;
        return Prop.Label(
            is403,
            $"Expected 403 for {endpoint.Method} {endpoint.Path}, got {(int)response.StatusCode}");
    }

    private static string GenerateReaderJwt()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestJwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("role", "reader"),
            new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
            new Claim("id", Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<HttpResponseMessage> SendAuthenticatedRequest(PublisherOnlyEndpoint endpoint, string jwt)
    {
        var request = new HttpRequestMessage(endpoint.HttpMethod, endpoint.Path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        return await _client.SendAsync(request);
    }
}

/// <summary>
/// Represents a publisher-only endpoint (HTTP method + path).
/// </summary>
public record PublisherOnlyEndpoint(string Method, string Path)
{
    public HttpMethod HttpMethod => new HttpMethod(Method);
}

/// <summary>
/// FsCheck arbitrary that generates all known publisher-only endpoints.
/// Each endpoint is drawn from the fixed list defined in the spec.
/// </summary>
public static class PublisherOnlyEndpointArbitrary
{
    private static readonly PublisherOnlyEndpoint[] PublisherOnlyEndpoints =
    [
        new("POST",   "/api/v1/stories/"),
        new("PATCH",  "/api/v1/stories/"),
        new("DELETE", "/api/v1/stories/"),
        new("DELETE", "/api/v1/stories/audio"),
        new("POST",   "/api/v1/media/documents/upload"),
        new("DELETE", "/api/v1/media/documents"),
    ];

    public static Arbitrary<PublisherOnlyEndpoint> Generate()
    {
        var gen = Gen.Elements<PublisherOnlyEndpoint>(PublisherOnlyEndpoints);
        return gen.ToArbitrary();
    }
}
