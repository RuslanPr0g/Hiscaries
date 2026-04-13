using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Hiscary.Shared.Domain.Authorization;
using Microsoft.AspNetCore.Http;
using System.Net;
using Xunit;

namespace Hiscary.Security.Tests.Authorization;

/// <summary>
/// Property-Based Tests for unauthenticated request authorization.
///
/// Validates: Requirements 2.1.6, 3.1.4, 4.1.3
///
/// Property: ∀ protected endpoint e, request without JWT → status 401
/// </summary>
public class UnauthenticatedRequestsReturn401Tests : IClassFixture<ProtectedEndpointsWebAppFactory>
{
    private readonly HttpClient _client;

    public UnauthenticatedRequestsReturn401Tests(ProtectedEndpointsWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    /// <summary>
    /// Validates: Requirements 2.1.6, 3.1.4, 4.1.3
    ///
    /// Property: ∀ protected endpoint e, request without JWT → status 401
    /// </summary>
    [Property(MaxTest = 1, Arbitrary = new[] { typeof(ProtectedEndpointArbitrary) })]
    public Property UnauthenticatedRequest_ToProtectedEndpoint_Returns401(ProtectedEndpoint endpoint)
    {
        var response = SendUnauthenticatedRequest(endpoint).GetAwaiter().GetResult();
        var is401 = response.StatusCode == HttpStatusCode.Unauthorized;
        return Prop.Label(
            is401,
            $"Expected 401 for {endpoint.Method} {endpoint.Path}, got {(int)response.StatusCode}");
    }

    private async Task<HttpResponseMessage> SendUnauthenticatedRequest(ProtectedEndpoint endpoint)
    {
        var request = new HttpRequestMessage(endpoint.HttpMethod, endpoint.Path);
        // No Authorization header — unauthenticated request
        return await _client.SendAsync(request);
    }
}

/// <summary>
/// Represents a protected endpoint (HTTP method + path).
/// </summary>
public record ProtectedEndpoint(string Method, string Path)
{
    public HttpMethod HttpMethod => new HttpMethod(Method);
}

/// <summary>
/// FsCheck arbitrary that generates all known protected endpoints.
/// Each endpoint is drawn from the fixed list defined in the spec.
/// </summary>
public static class ProtectedEndpointArbitrary
{
    private static readonly ProtectedEndpoint[] ProtectedEndpoints =
    [
        // Stories — RequirePublisher
        new("POST",   "/api/v1/stories/"),
        new("PATCH",  "/api/v1/stories/"),
        new("DELETE", "/api/v1/stories/"),
        new("DELETE", "/api/v1/stories/audio"),
        // Stories — RequireReaderOrAbove
        new("DELETE", "/api/v1/stories/comments"),
        new("PATCH",  "/api/v1/stories/comments"),
        // Media — RequirePublisher
        new("POST",   "/api/v1/media/documents/upload"),
        new("DELETE", "/api/v1/media/documents"),
        // Media — RequireAuthorization
        new("GET",    "/api/v1/media/images/test.jpg"),
        new("GET",    "/api/v1/media/documents/test.pdf"),
        // PlatformUsers — RequireAuthorization
        new("POST",   "/api/v1/users/read"),
        new("POST",   "/api/v1/users/bookmark"),
    ];

    public static Arbitrary<ProtectedEndpoint> Generate()
    {
        var gen = Gen.Elements<ProtectedEndpoint>(ProtectedEndpoints);
        return gen.ToArbitrary();
    }
}
