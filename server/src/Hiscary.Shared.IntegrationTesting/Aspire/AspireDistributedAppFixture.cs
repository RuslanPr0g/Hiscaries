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

    protected TimeSpan DefaultTimeOut = TimeSpan.FromMinutes(5);

    protected virtual string[] AppHostArgs => ["UseVolumes=false", "--environment=Development"];

    public async Task InitializeAsync()
    {
        var appHostBuilder = await DistributedApplicationTestingBuilder
            .CreateAsync<TEntryPoint>(AppHostArgs);

        _app = await appHostBuilder.BuildAsync().WaitAsync(DefaultTimeOut);
        await _app.StartAsync().WaitAsync(DefaultTimeOut);
    }

    public async Task DisposeAsync()
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
