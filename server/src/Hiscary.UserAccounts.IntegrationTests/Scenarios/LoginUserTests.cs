using Hiscary.Shared.IntegrationTesting.Aspire;
using Hiscary.Shared.IntegrationTesting.Assertions;
using Hiscary.Shared.IntegrationTesting.Http;
using Hiscary.UserAccounts.Api.Rest.Requests;
using StackNucleus.DDD.Domain.ResultModels;
using Xunit;

namespace Hiscary.UserAccounts.IntegrationTests.Scenarios;

public sealed class LoginUserTests(UserAccountsAppHostFixture fixture) : IClassFixture<UserAccountsAppHostFixture>
{
    [Fact]
    public async Task LoginUser_WithValidCredentials_ReturnsTokenMetadata_WhenUserIsRegistered()
    {
        // TODO: might be better to create http client once per fixture
        using var client = await fixture.CreateReadyUserAccountsClientAsync();
        var username = $"integration_{Guid.NewGuid():N}";
        const string password = "integration_password";

        var registerResponse = await client.PostJsonAsync("/api/v1/accounts/register", new RegisterUserRequest
        {
            Username = username,
            Email = $"{username}@integration.test",
            Password = password,
            BirthDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
        registerResponse.AssertSuccess();

        var loginResponse = await client.PostJsonAsync("/api/v1/accounts/login", new UserLoginRequest
        {
            Username = username,
            Password = password
        });
        loginResponse.AssertSuccess();

        var payload = await loginResponse.ReadRequiredJsonAsync<TokenMetadata>();
        Assert.False(string.IsNullOrWhiteSpace(payload.Token));
        Assert.False(string.IsNullOrWhiteSpace(payload.RefreshToken));
    }
}

public sealed class UserAccountsAppHostFixture : AspireDistributedAppFixture<Projects.Hiscary_AppHost>
{
    public Task<HttpClient> CreateReadyUserAccountsClientAsync()
    {
        return CreateHttpClientForResource("hc-useraccounts-api-rest");
    }

    protected override string[] AppHostArgs =>
    [
        "UseVolumes=false",
        "--environment=Development"
    ];
}
