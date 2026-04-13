/// <summary>
/// Minimal entry point class used by WebApplicationFactory to create a test host.
/// The actual app configuration is done in ProtectedEndpointsWebAppFactory.ConfigureWebHost.
/// </summary>
public partial class TestProgram
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();
        app.Run();
    }
}
