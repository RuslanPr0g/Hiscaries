namespace Hiscary.Shared.Domain.Authorization;

public static class AuthorizationPolicies
{
    public const string RequirePublisher = "RequirePublisher";
    public const string RequireAdmin = "RequireAdmin";
    public const string RequireReaderOrAbove = "RequireReaderOrAbove";
    public const string RoleClaimType = "role";
}
