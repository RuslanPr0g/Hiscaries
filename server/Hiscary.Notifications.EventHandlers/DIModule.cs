﻿using Hiscary.Notifications.DomainEvents;
using Hiscary.Notifications.EventHandlers.DomainEvents;
using Hiscary.Notifications.EventHandlers.IntegrationEvents;
using Hiscary.Notifications.IntegrationEvents.Incoming;
using Hiscary.PlatformUsers.IntegrationEvents.Outgoing;
using Hiscary.EventHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.Hosting;

namespace Hiscary.Notifications.EventHandlers;

public static class DIModule
{
    public static void AddEventHandlers(
        this IHostApplicationBuilder builder,
        IConfiguration configuration)
    {
        builder.Services.AddScoped<IEventHandler<NotificationCreatedDomainEvent>, NotificationCreatedDomainEventHandler>();
        builder.Services.AddScoped<IEventHandler<UserPublishedStoryIntegrationEvent>, UserPublishedStoryIntegrationEventHandler>();
        builder.Services.AddScoped<IEventHandler<NotificationReferenceObjectIdPreviewChangedIntegrationEvent>,
            NotificationReferenceObjectIdPreviewChangedIntegrationEventHandler>();

        var asm = Assembly.GetExecutingAssembly();
        var rabbitMqConnectionString = configuration.GetConnectionString("rabbitmq");
        ArgumentException.ThrowIfNullOrWhiteSpace(rabbitMqConnectionString);

        builder.AddConfigurableEventHandlers([asm], rabbitMqConnectionString, "notification-events-queue");
    }
}