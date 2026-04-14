using Hiscary.Media.IntegrationEvents;
using Hiscary.PlatformUsers.DomainEvents;
using Hiscary.PlatformUsers.IntegrationEvents;
using Hiscary.PlatformUsers.Persistence.Context;
using StackNucleus.DDD.Domain.EventPublishers;
using StackNucleus.DDD.Outbox.Jobs;
using System.Reflection;

namespace Hiscary.PlatformUsers.Jobs;

internal class ProcessOutboxMessagesJob(
    PlatformUsersContext context,
    IEventPublisher publisher) : BaseProcessOutboxMessagesJob<PlatformUsersContext, Assembly>(publisher)
{
    protected override PlatformUsersContext Context { get; init; } = context;
    protected override IReadOnlyList<Assembly> MessagesAssembly { get; init; } =
        [
        typeof(PlatformUserDomainEventsAssembly).Assembly,
        typeof(PlatformUserIntegrationEventsAssembly).Assembly,
        typeof(MediaIntegrationEventsAssembly).Assembly];
}
