using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Xunit;

namespace Hiscary.Shared.IntegrationTesting.Aspire;

public abstract class AspireDistributedAppFixture<TEntryPoint> : IAsyncLifetime
    where TEntryPoint : class
{
    private DistributedApplication? _app;

    protected DistributedApplication App =>
        _app ?? throw new InvalidOperationException("AppHost is not initialized.");

    protected TimeSpan DefaultTimeOut = TimeSpan.FromMinutes(10);

    protected virtual string[] AppHostArgs =>
        ["UseVolumes=false", "--environment=Development"];

    public async ValueTask InitializeAsync()
    {
        var appHostBuilder = await DistributedApplicationTestingBuilder
            .CreateAsync<TEntryPoint>(AppHostArgs);

        _app = await appHostBuilder.BuildAsync().WaitAsync(DefaultTimeOut);
        await _app.StartAsync().WaitAsync(DefaultTimeOut);
    }

    public async ValueTask DisposeAsync()
    {
        if (_app is not null)
        {
            await _app.DisposeAsync();
        }
    }

    // CreateHttpClient(resourceName) alone resolves the endpoint named "http" by
    // default; every API resource here only exposes an endpoint named "rest"
    // (WithHttpsEndpoint(name: "rest", ...)), so the default client silently
    // targets a nonexistent endpoint and the resiliency handler retries until
    // HttpClient.Timeout instead of failing fast. Always pass the endpoint name.
    //
    // WaitForResourceHealthyAsync only blocks until the AppHost's own health
    // check reports healthy. Project resources need an explicit
    // .WithHttpHealthCheck(...) in the AppHost, otherwise DCP marks them
    // "Healthy" the instant the OS process spawns, before Kestrel has bound
    // its port, and this call returns a client pointed at a server that
    // isn't listening yet.
    protected async Task<HttpClient> CreateHttpClientForResource(
        string resourceName, string endpointName = "rest", TimeSpan? timeout = null)
    {
        await App.ResourceNotifications.WaitForResourceHealthyAsync(resourceName).WaitAsync(DefaultTimeOut);
        var client = App.CreateHttpClient(resourceName, endpointName);
        client.Timeout = timeout ?? TimeSpan.FromMinutes(2);
        return client;
    }
}
