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

    // 5 minutes was too tight for a cold CI runner: StartAsync must pull ~4.4GB
    // across 6 container images (Postgres, RabbitMQ, Elasticsearch, Redis,
    // Azurite, PgAdmin) with no layer cache before resources become healthy.
    protected TimeSpan DefaultTimeOut = TimeSpan.FromMinutes(12);

    protected virtual string[] AppHostArgs => ["UseVolumes=false", "--environment=Development"];

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

    protected async Task<HttpClient> CreateHttpClientForResource(string resourceName, TimeSpan? timeout = null)
    {
        var client = App.CreateHttpClient(resourceName);
        await App.ResourceNotifications.WaitForResourceHealthyAsync(resourceName).WaitAsync(DefaultTimeOut);
        client.Timeout = timeout ?? TimeSpan.FromMinutes(2);
        return client;
    }
}
