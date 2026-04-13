namespace Hiscary.IntegrationTesting.Security;

public static class SecurityEndpointCatalog
{
    public static readonly EndpointDescriptor[] ProtectedEndpoints =
    [
        new("POST",   "/api/v1/stories/"),
        new("PATCH",  "/api/v1/stories/"),
        new("DELETE", "/api/v1/stories/"),
        new("DELETE", "/api/v1/stories/audio"),
        new("DELETE", "/api/v1/stories/comments"),
        new("PATCH",  "/api/v1/stories/comments"),
        new("POST",   "/api/v1/media/documents/upload"),
        new("DELETE", "/api/v1/media/documents"),
        new("GET",    "/api/v1/media/images/test.jpg"),
        new("GET",    "/api/v1/media/documents/test.pdf"),
        new("POST",   "/api/v1/users/read"),
        new("POST",   "/api/v1/users/bookmark")
    ];

    public static readonly EndpointDescriptor[] PublisherOnlyEndpoints =
    [
        new("POST",   "/api/v1/stories/"),
        new("PATCH",  "/api/v1/stories/"),
        new("DELETE", "/api/v1/stories/"),
        new("DELETE", "/api/v1/stories/audio"),
        new("POST",   "/api/v1/media/documents/upload"),
        new("DELETE", "/api/v1/media/documents")
    ];
}

public sealed record EndpointDescriptor(string Method, string Path)
{
    public HttpMethod HttpMethod => new(Method);
}
