using Hiscary.Shared.Domain.Authorization;
using Hiscary.Shared.Domain.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Hiscary.Security.Tests.Authorization;

/// <summary>
/// A minimal WebApplicationFactory that creates a test ASP.NET Core app
/// with the same JWT authentication and authorization policies as the real microservices.
///
/// This factory registers all protected endpoints from the spec with their
/// corresponding authorization policies, allowing us to verify that
/// unauthenticated requests are rejected with 401 before reaching any handler.
/// </summary>
public class ProtectedEndpointsWebAppFactory : WebApplicationFactory<TestProgram>
{
    // Test JWT settings — used only in tests, not real credentials
    private const string TestJwtKey = "test-security-key-for-hiscary-rbac-pbt-tests-32chars";
    private const string TestIssuer = "hiscary-test";
    private const string TestAudience = "hiscary-test";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseTestServer();
        builder.Configure(ConfigureApp);
        builder.ConfigureServices(ConfigureServices);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = TestIssuer,
            ValidAudience = TestAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestJwtKey)),
            ClockSkew = TimeSpan.Zero
        };

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = tokenValidationParameters;
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicies.RequirePublisher, policy =>
                policy.RequireClaim(AuthorizationPolicies.RoleClaimType, "publisher", "admin"));

            options.AddPolicy(AuthorizationPolicies.RequireAdmin, policy =>
                policy.RequireClaim(AuthorizationPolicies.RoleClaimType, "admin"));

            options.AddPolicy(AuthorizationPolicies.RequireReaderOrAbove, policy =>
                policy.RequireClaim(AuthorizationPolicies.RoleClaimType, "reader", "publisher", "admin"));
        });

        services.AddRouting();
    }

    private static void ConfigureApp(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            // ── Stories microservice endpoints ──────────────────────────────

            // RequirePublisher
            endpoints.MapPost("/api/v1/stories/", () => Results.Ok())
                .RequireAuthorization(AuthorizationPolicies.RequirePublisher);

            endpoints.MapMethods("/api/v1/stories/", ["PATCH"], () => Results.Ok())
                .RequireAuthorization(AuthorizationPolicies.RequirePublisher);

            endpoints.MapDelete("/api/v1/stories/", () => Results.Ok())
                .RequireAuthorization(AuthorizationPolicies.RequirePublisher);

            endpoints.MapDelete("/api/v1/stories/audio", () => Results.Ok())
                .RequireAuthorization(AuthorizationPolicies.RequirePublisher);

            // RequireReaderOrAbove
            endpoints.MapDelete("/api/v1/stories/comments", () => Results.Ok())
                .RequireAuthorization(AuthorizationPolicies.RequireReaderOrAbove);

            endpoints.MapMethods("/api/v1/stories/comments", ["PATCH"], () => Results.Ok())
                .RequireAuthorization(AuthorizationPolicies.RequireReaderOrAbove);

            // ── Media microservice endpoints ────────────────────────────────

            // RequireAuthorization (any authenticated user)
            endpoints.MapGet("/api/v1/media/images/{fileName}", (string fileName) => Results.Ok())
                .RequireAuthorization();

            endpoints.MapGet("/api/v1/media/documents/{fileName}", (string fileName) => Results.Ok())
                .RequireAuthorization();

            // RequirePublisher
            endpoints.MapPost("/api/v1/media/documents/upload", () => Results.Ok())
                .RequireAuthorization(AuthorizationPolicies.RequirePublisher);

            endpoints.MapDelete("/api/v1/media/documents", () => Results.Ok())
                .RequireAuthorization(AuthorizationPolicies.RequirePublisher);

            // ── PlatformUsers microservice endpoints ────────────────────────

            // RequireAuthorization (any authenticated user)
            endpoints.MapPost("/api/v1/users/read", () => Results.Ok())
                .RequireAuthorization();

            endpoints.MapPost("/api/v1/users/bookmark", () => Results.Ok())
                .RequireAuthorization();
        });
    }
}
