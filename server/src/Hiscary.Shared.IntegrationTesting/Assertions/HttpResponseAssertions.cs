using Xunit;

namespace Hiscary.IntegrationTesting.Assertions;

public static class HttpResponseAssertions
{
    public static void AssertSuccess(this HttpResponseMessage response)
    {
        Assert.True(response.IsSuccessStatusCode, $"Expected success status code, got {(int)response.StatusCode} ({response.StatusCode}).");
    }
}
