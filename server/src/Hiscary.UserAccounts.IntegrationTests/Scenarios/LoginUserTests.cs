using System.Net.Http.Json;
using Hiscary.IntegrationTesting.Aspire;
using Hiscary.IntegrationTesting.Http;
using Xunit;

namespace Hiscary.UserAccounts.IntegrationTests.Scenarios;

public sealed class LoginUserTests(UserAccountsAppHostFixture fixture) : IClassFixture<UserAccountsAppHostFixture>
{
    [Fact]
    public async Task LoginUser_WithValidCredentials_ReturnsTokenMetadata_WhenUserIsRegistered()
    {
        using var client = fixture.CreateUserAccountsClient();
        var username = $"integration_{Guid.NewGuid():N}";
        const string password = "integration_password";

        var registerResponse = await client.PostAsJsonAsync("/api/v1/accounts/register", new RegisterUserRequest(
            Username: username,
            Email: $"{username}@integration.test",
            Password: password,
            BirthDate: new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
        Assert.Equal(System.Net.HttpStatusCode.Created, registerResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync("/api/v1/accounts/login", new UserLoginRequest(
            Username: username,
            Password: password));

        var payload = await loginResponse.ReadRequiredJsonAsync<TokenMetadataResponse>();
        Assert.False(string.IsNullOrWhiteSpace(payload.Token));
        Assert.False(string.IsNullOrWhiteSpace(payload.RefreshToken));
    }

    private sealed record RegisterUserRequest(
        string Username,
        string Email,
        string Password,
        DateTime BirthDate);

    private sealed record UserLoginRequest(
        string Username,
        string Password);

    private sealed record TokenMetadataResponse(
        string Token,
        string RefreshToken);
}

public sealed class UserAccountsAppHostFixture : AspireDistributedAppFixture<Projects.Hiscary_AppHost>
{
    public HttpClient CreateUserAccountsClient()
    {
        var client = CreateHttpClientForResource("hc-useraccounts-api-rest");
        client.Timeout = TimeSpan.FromSeconds(30);
        return client;
    }

    protected override string[] AppHostArgs =>
    [
        "UseVolumes=false",
        "--environment=Development"
    ];
}
