using Hiscary.Shared.Domain.Authorization;
using Hiscary.Shared.IntegrationTesting.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Hiscary.Security.IntegrationTests.Authorization;

public class ProtectedEndpointsWebAppFactory : WebApplicationFactory<TestProgram>
{
    public const string TestJwtKey = "test-security-key-for-hiscary-rbac-pbt-tests-32chars";
    public const string TestIssuer = "hiscary-test";
    public const string TestAudience = "hiscary-test";

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
            endpoints.MapPost("/api/v1/stories/", () => Results.Ok())
                .RequireAuthorization(AuthorizationPolicies.RequirePublisher);
            endpoints.MapMethods("/api/v1/stories/", ["PATCH"], () => Results.Ok())
                .RequireAuthorization(AuthorizationPolicies.RequirePublisher);
            endpoints.MapDelete("/api/v1/stories/", () => Results.Ok())
                .RequireAuthorization(AuthorizationPolicies.RequirePublisher);
            endpoints.MapDelete("/api/v1/stories/audio", () => Results.Ok())
                .RequireAuthorization(AuthorizationPolicies.RequirePublisher);
            endpoints.MapDelete("/api/v1/stories/comments", () => Results.Ok())
                .RequireAuthorization(AuthorizationPolicies.RequireReaderOrAbove);
            endpoints.MapMethods("/api/v1/stories/comments", ["PATCH"], () => Results.Ok())
                .RequireAuthorization(AuthorizationPolicies.RequireReaderOrAbove);

            endpoints.MapGet("/api/v1/media/images/{fileName}", (string fileName) => Results.Ok())
                .RequireAuthorization();
            endpoints.MapGet("/api/v1/media/documents/{fileName}", (string fileName) => Results.Ok())
                .RequireAuthorization();
            endpoints.MapPost("/api/v1/media/documents/upload", () => Results.Ok())
                .RequireAuthorization(AuthorizationPolicies.RequirePublisher);
            endpoints.MapDelete("/api/v1/media/documents", () => Results.Ok())
                .RequireAuthorization(AuthorizationPolicies.RequirePublisher);

            endpoints.MapPost("/api/v1/users/read", () => Results.Ok())
                .RequireAuthorization();
            endpoints.MapPost("/api/v1/users/bookmark", () => Results.Ok())
                .RequireAuthorization();
        });
    }

    public JwtTokenFactory CreateJwtFactory() => new(TestJwtKey, TestIssuer, TestAudience);
}
