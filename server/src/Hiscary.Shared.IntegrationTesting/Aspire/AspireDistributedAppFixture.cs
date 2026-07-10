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

    protected TimeSpan DefaultTimeOut = TimeSpan.FromMinutes(7);

    protected virtual string[] AppHostArgs => ["UseVolumes=false", "--environment=Development"];

    private static readonly string DiagPath = Path.Combine(
        AppContext.BaseDirectory, "aspire-fixture-diag.log");

    private static void DiagLog(string message) =>
        File.AppendAllText(DiagPath, $"{DateTime.UtcNow:O} {message}{Environment.NewLine}");

    private static readonly HashSet<string> DumpedLogsFor = [];

    private static async Task DumpContainerLogsOnceAsync(string resourceName)
    {
        if (!DumpedLogsFor.Add(resourceName))
        {
            return;
        }

        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo("docker", $"ps -a --filter \"name={resourceName}\" --format \"{{{{.Names}}}}\"")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };
            using var listProc = System.Diagnostics.Process.Start(psi)!;
            var containerNames = (await listProc.StandardOutput.ReadToEndAsync()).Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            await listProc.WaitForExitAsync();

            foreach (var containerName in containerNames)
            {
                var logsPsi = new System.Diagnostics.ProcessStartInfo("docker", $"logs {containerName}")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                };
                using var logsProc = System.Diagnostics.Process.Start(logsPsi)!;
                var stdout = await logsProc.StandardOutput.ReadToEndAsync();
                var stderr = await logsProc.StandardError.ReadToEndAsync();
                await logsProc.WaitForExitAsync();
                DiagLog($"[ContainerLogs:{containerName}] stdout=\n{stdout}\nstderr=\n{stderr}");
            }
        }
        catch (Exception ex)
        {
            DiagLog($"[ContainerLogs:{resourceName}] failed to capture: {ex}");
        }
    }

    public async ValueTask InitializeAsync()
    {
        var appHostBuilder = await DistributedApplicationTestingBuilder
            .CreateAsync<TEntryPoint>(AppHostArgs);

        _app = await appHostBuilder.BuildAsync().WaitAsync(DefaultTimeOut);

        using var watchCts = new CancellationTokenSource();
        var watchTask = Task.Run(async () =>
        {
            try
            {
                await foreach (var evt in _app.ResourceNotifications.WatchAsync(watchCts.Token))
                {
                    DiagLog($"[ResourceWatch] {evt.Resource.Name} -> {evt.Snapshot.State?.Text} (health: {evt.Snapshot.HealthStatus})");
                    if (evt.Snapshot.State?.Text is "Exited" or "FailedToStart")
                    {
                        await DumpContainerLogsOnceAsync(evt.Resource.Name);
                    }
                }
            }
            catch (OperationCanceledException) { }
        }, CancellationToken.None);

        try
        {
            await _app.StartAsync().WaitAsync(DefaultTimeOut);
        }
        finally
        {
            watchCts.Cancel();
            await watchTask;
        }
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
