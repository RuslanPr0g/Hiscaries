﻿using Hiscary.Media.EventHandlers.IntegrationEvents;
using Hiscary.Media.IntegrationEvents.Incoming;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackNucleus.DDD.Domain.EventHandlers;
using StackNucleus.DDD.Events.WolverineFx;
using System.Reflection;

namespace Hiscary.Media.EventHandlers;

public static class DIModule
{
    public static void AddEventHandlers(
        this IHostApplicationBuilder builder,
        IConfiguration configuration)
    {
        builder.Services.AddScoped<IEventHandler<ImageUploadRequestedIntegrationEvent>, ImageUploadRequestedIntegrationEventHandler>();

        var asm = Assembly.GetExecutingAssembly();
        var rabbitMqConnectionString = configuration.GetConnectionString("rabbitmq");
        ArgumentException.ThrowIfNullOrWhiteSpace(rabbitMqConnectionString);

        // TODO: remove this when apire team understands that we REALLY need to wait for some resources inside docker
        Thread.Sleep(5000);

        builder.AddConfigurableEventHandlers([asm], rabbitMqConnectionString, "media-events-queue");
    }
}
