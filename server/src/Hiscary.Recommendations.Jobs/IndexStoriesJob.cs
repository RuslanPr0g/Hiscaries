using Hiscary.Recommendations.Domain.Persistence.Read;
using Hiscary.Stories.IntegrationEvents.Incoming;
using Quartz;
using StackNucleus.DDD.Domain.EventPublishers;

namespace Hiscary.Recommendations.Jobs;

internal sealed class IndexStoriesJob(
    IEventPublisher publisher,
    ISystemDataAvailabilityRepository availabilityRepository) : IJob
{
    private readonly IEventPublisher _publisher = publisher;
    private readonly ISystemDataAvailabilityRepository _availabilityRepository = availabilityRepository;

    public async Task Execute(IJobExecutionContext context)
    {
        var isUserDataAvailable = await _availabilityRepository.IsUserDataAvailable(context.CancellationToken);
        var isStoryDataAvailable = await _availabilityRepository.IsStoryDataAvailable(context.CancellationToken);

        if (!isUserDataAvailable)
        {
            await _availabilityRepository.CreateUserIndex(context.CancellationToken);
        }

        if (!isStoryDataAvailable)
        {
            await _availabilityRepository.CreateStoryIndex(context.CancellationToken);
        }

        if (!isStoryDataAvailable)
        {
            // TODO: do not allow multiple instances to call this at the same time, track the status of the current data roll out
            // per requester id
            // TODO: the requester id should be used in the StoryInformationRefreshChunkProcessedIntegrationEventHandler
            // to check that it's indeed the recommendation service calling it
            // for this we need to store the guid of the service in a configuration file
            await _publisher.Publish(new StoryInformationRefreshRequestedIntegrationEvent(
                Guid.NewGuid(),
                0,
                150));
        }
    }
}