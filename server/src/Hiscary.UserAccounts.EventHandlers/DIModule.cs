using Hiscary.PlatformUsers.IntegrationEvents.Outgoing;
using Hiscary.UserAccounts.EventHandlers.IntegrationEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackNucleus.DDD.Domain.EventHandlers;
using StackNucleus.DDD.Events.WolverineFx;
using System.Reflection;

namespace Hiscary.UserAccounts.EventHandlers;

public static class DIModule
{
    public static void AddEventHandlers(
        this IHostApplicationBuilder builder,
        IConfiguration configuration)
    {
        builder.Services.AddScoped<IEventHandler<UserBecamePublisherIntegrationEvent>, UserBecamePublisherIntegrationEventHandler>();

        var asm = Assembly.GetExecutingAssembly();
        var rabbitMqConnectionString = configuration.GetConnectionString("rabbitmq");
        ArgumentException.ThrowIfNullOrWhiteSpace(rabbitMqConnectionString);

        builder.AddConfigurableEventHandlers([asm], rabbitMqConnectionString, "useraccount-events-queue");
    }
}