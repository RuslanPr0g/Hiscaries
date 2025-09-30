using Hiscary.Recommendations.EventHandlers.IntegrationEvents;
using Hiscary.Stories.IntegrationEvents.Outgoing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackNucleus.DDD.Domain.EventHandlers;
using StackNucleus.DDD.Events.WolverineFx;
using System.Reflection;

namespace Hiscary.Recommendations.EventHandlers;

public static class DIModule
{
    public static void AddEventHandlers(
        this IHostApplicationBuilder builder,
        IConfiguration configuration)
    {
        builder.Services.AddScoped<IEventHandler<StoryPublishedIntegrationEvent>, StoryPublishedIntegrationEventHandler>();
        builder.Services.AddScoped<IEventHandler<StoryUpdatedIntegrationEvent>, StoryUpdatedIntegrationEventHandler>();
        builder.Services.AddScoped<IEventHandler<StoryUniqueReadCountIncreasedIntegrationEvent>, StoryUniqueReadCountIncreasedIntegrationEventHandler>();
        builder.Services.AddScoped<IEventHandler<StoryInformationRefreshChunkProcessedIntegrationEvent>, StoryInformationRefreshChunkProcessedIntegrationEventHandler>();

        var asm = Assembly.GetExecutingAssembly();
        var rabbitMqConnectionString = configuration.GetConnectionString("rabbitmq");
        ArgumentException.ThrowIfNullOrWhiteSpace(rabbitMqConnectionString);

        // TODO: remove this when apire team understands that we REALLY need to wait for some resources inside docker
        Thread.Sleep(15000);

        builder.AddConfigurableEventHandlers([asm], rabbitMqConnectionString, "recommendation-events-queue");
    }
}