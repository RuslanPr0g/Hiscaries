using Hiscary.Shared.Domain.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Hiscary.Shared.Application.Extensions;

public static class AuthorizationPoliciesConfiguration
{
    public static IServiceCollection AddAuthorizationPolicies(
        this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicies.RequirePublisher, policy =>
                policy.RequireClaim(AuthorizationPolicies.RoleClaimType, "publisher", "admin"));

            options.AddPolicy(AuthorizationPolicies.RequireAdmin, policy =>
                policy.RequireClaim(AuthorizationPolicies.RoleClaimType, "admin"));

            options.AddPolicy(AuthorizationPolicies.RequireReaderOrAbove, policy =>
                policy.RequireClaim(AuthorizationPolicies.RoleClaimType, "reader", "publisher", "admin"));
        });

        return services;
    }
}
