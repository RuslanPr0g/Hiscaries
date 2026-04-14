namespace Hiscary.Shared.Domain.Authorization;

public static class AuthorizationPolicies
{
    public const string RequirePublisher = "RequirePublisher";
    public const string RequireAdmin = "RequireAdmin";
    public const string RequireReaderOrAbove = "RequireReaderOrAbove";
    public const string FullRoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
    public const string RoleClaimType = "role";
}
