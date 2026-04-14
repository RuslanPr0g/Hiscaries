using Hiscary.Shared.IntegrationTesting.Aspire;
using Hiscary.Shared.IntegrationTesting.Assertions;
using Hiscary.Shared.IntegrationTesting.Http;
using Hiscary.UserAccounts.Api.Rest.Requests;
using Xunit;

namespace Hiscary.UserAccounts.IntegrationTests.Scenarios;

public sealed class LoginUserTests(UserAccountsAppHostFixture fixture) : IClassFixture<UserAccountsAppHostFixture>
{
    [Fact]
    public async Task LoginUser_WithValidCredentials_ReturnsTokenMetadata_WhenUserIsRegistered()
    {
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

        var payload = await loginResponse.ReadRequiredJsonAsync<TokenMetadataResponse>();
        Assert.False(string.IsNullOrWhiteSpace(payload.Token));
        Assert.False(string.IsNullOrWhiteSpace(payload.RefreshToken));
    }

    private sealed record TokenMetadataResponse(
        string Token,
        string RefreshToken);
}

public sealed class UserAccountsAppHostFixture : AspireDistributedAppFixture<Projects.Hiscary_AppHost>
{
    public async Task<HttpClient> CreateReadyUserAccountsClientAsync()
    {
        var client = CreateHttpClientForResource("hc-useraccounts-api-rest");
        client.Timeout = TimeSpan.FromMinutes(5);
        await WaitUntilHealthyAsync(client, TimeSpan.FromMinutes(8));
        return client;
    }

    private static async Task WaitUntilHealthyAsync(HttpClient client, TimeSpan timeout)
    {
        var startedAt = DateTime.UtcNow;

        while (DateTime.UtcNow - startedAt < timeout)
        {
            try
            {
                using var response = await client.GetAsync("/health");
                if ((int)response.StatusCode < 500)
                {
                    return;
                }
            }
            catch
            {
                // Service may still be starting up; retry.
            }

            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        throw new TimeoutException("UserAccounts service did not become healthy in time.");
    }

    protected override string[] AppHostArgs =>
    [
        "UseVolumes=false",
        "--environment=Development"
    ];
}
