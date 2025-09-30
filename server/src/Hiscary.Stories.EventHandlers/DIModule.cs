using Hiscary.Media.IntegrationEvents.Outgoing;
using Hiscary.PlatformUsers.IntegrationEvents.Outgoing;
using Hiscary.Stories.EventHandlers.IntegrationEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackNucleus.DDD.Domain.EventHandlers;
using StackNucleus.DDD.Events.WolverineFx;
using System.Reflection;

namespace Hiscary.Stories.EventHandlers;

public static class DIModule
{
    public static void AddEventHandlers(
        this IHostApplicationBuilder builder,
        IConfiguration configuration)
    {
        builder.Services.AddScoped<IEventHandler<ImageUploadedIntegrationEvent>, ImageUploadedIntegrationEventHandler>();
        builder.Services.AddScoped<IEventHandler<StoryFirstReadIntegrationEvent>, StoryFirstReadIntegrationEventHandler>();

        var asm = Assembly.GetExecutingAssembly();
        var rabbitMqConnectionString = configuration.GetConnectionString("rabbitmq");
        ArgumentException.ThrowIfNullOrWhiteSpace(rabbitMqConnectionString);

        // TODO: remove this when apire team understands that we REALLY need to wait for some resources inside docker
        Thread.Sleep(15000);

        builder.AddConfigurableEventHandlers([asm], rabbitMqConnectionString, "story-events-queue");
    }
}