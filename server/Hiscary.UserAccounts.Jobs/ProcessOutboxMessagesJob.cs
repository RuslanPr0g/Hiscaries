﻿using StackNucleus.DDD.Domain.EventPublishers;
using Hiscary.Outbox;
using Hiscary.UserAccounts.IntegrationEvents;
using Hiscary.UserAccounts.Persistence.Context;
using System.Reflection;

namespace Hiscary.UserAccounts.Jobs;

internal class ProcessOutboxMessagesJob(
    UserAccountsContext context,
    IEventPublisher publisher) : BaseProcessOutboxMessagesJob<UserAccountsContext, Assembly>(publisher)
{
    protected override UserAccountsContext Context { get; init; } = context;
    protected override IReadOnlyList<Assembly> MessagesAssembly { get; init; } = 
        [typeof(UserAccountIntegrationEventsAssembly).Assembly];
}
