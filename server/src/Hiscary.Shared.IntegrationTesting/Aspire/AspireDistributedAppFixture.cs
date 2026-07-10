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

    // Temporary: diagnosing why StartAsync times out in CI (works locally).
    // Logs resource state/health transitions so we can tell "still pulling
    // images" apart from "stuck forever waiting on a health check". Remove
    // once root cause for the CI-only timeout is confirmed.
    private static readonly string DiagPath = Path.Combine(
        AppContext.BaseDirectory, "aspire-fixture-diag.log");

    private CancellationTokenSource? _watchCts;
    private Task? _watchTask;

    public async ValueTask InitializeAsync()
    {
        var appHostBuilder = await DistributedApplicationTestingBuilder
            .CreateAsync<TEntryPoint>(AppHostArgs);

        _app = await appHostBuilder.BuildAsync().WaitAsync(DefaultTimeOut);

        _watchCts = new CancellationTokenSource();
        _watchTask = Task.Run(async () =>
        {
            try
            {
                await foreach (var evt in _app.ResourceNotifications.WatchAsync(_watchCts.Token))
                {
                    File.AppendAllText(DiagPath,
                        $"{DateTime.UtcNow:O} {evt.Resource.Name} -> state={evt.Snapshot.State?.Text} health={evt.Snapshot.HealthStatus}{Environment.NewLine}");
                }
            }
            catch (OperationCanceledException) { }
        }, CancellationToken.None);

        await _app.StartAsync().WaitAsync(DefaultTimeOut);
    }

    public async ValueTask DisposeAsync()
    {
        if (_watchCts is not null)
        {
            _watchCts.Cancel();
            if (_watchTask is not null)
            {
                await _watchTask;
            }
        }

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
