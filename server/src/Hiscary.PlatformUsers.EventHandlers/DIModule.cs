using Hiscary.Media.IntegrationEvents.Outgoing;
using Hiscary.PlatformUsers.DomainEvents;
using Hiscary.PlatformUsers.EventHandlers.DomainEvents;
using Hiscary.PlatformUsers.EventHandlers.IntegrationEvents;
using Hiscary.Stories.IntegrationEvents.Outgoing;
using Hiscary.UserAccounts.IntegrationEvents.Outgoing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackNucleus.DDD.Domain.EventHandlers;
using StackNucleus.DDD.Events.WolverineFx;
using System.Reflection;

namespace Hiscary.PlatformUsers.EventHandlers;

public static class DIModule
{
    public static void AddEventHandlers(
        this IHostApplicationBuilder builder,
        IConfiguration configuration)
    {
        builder.Services.AddScoped<IEventHandler<StoryPublishedIntegrationEvent>, StoryPublishedIntegrationEventHandler>();
        builder.Services.AddScoped<IEventHandler<UserAccountCreatedIntegrationEvent>, UserAccountCreatedIntegrationEventHandler>();
        builder.Services.AddScoped<IEventHandler<ImageUploadedIntegrationEvent>, ImageUploadedIntegrationEventHandler>();
        builder.Services.AddScoped<IEventHandler<UserFirstReadStoryDomainEvent>, UserFirstReadStoryDomainEventHandler>();

        var asm = Assembly.GetExecutingAssembly();
        var rabbitMqConnectionString = configuration.GetConnectionString("rabbitmq");
        ArgumentException.ThrowIfNullOrWhiteSpace(rabbitMqConnectionString);

        builder.AddConfigurableEventHandlers([asm], rabbitMqConnectionString, "platformuser-events-queue");
    }
}