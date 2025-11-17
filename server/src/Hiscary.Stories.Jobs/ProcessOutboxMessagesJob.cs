using Hiscary.Media.IntegrationEvents;
using Hiscary.PlatformUsers.DomainEvents;
using Hiscary.Stories.IntegrationEvents;
using Hiscary.Stories.IntegrationEvents.Outgoing;
using Hiscary.Stories.Persistence.Context;
using StackNucleus.DDD.Domain.EventPublishers;
using StackNucleus.DDD.Outbox.Jobs;
using System.Reflection;

namespace Hiscary.Stories.Jobs;

internal class ProcessOutboxMessagesJob(
    StoriesContext context,
    IEventPublisher publisher) : BaseProcessOutboxMessagesJob<StoriesContext, Assembly>(publisher)
{
    protected override StoriesContext Context { get; init; } = context;
    protected override IReadOnlyList<Assembly> MessagesAssembly { get; init; } =
        [
        typeof(StoryContentsChangedDomainEvent).Assembly,
        typeof(StoryContentsChangedIntegrationEvent).Assembly,
        typeof(StoryIntegrationEventsAssembly).Assembly,
        typeof(MediaIntegrationEventsAssembly).Assembly];
}
