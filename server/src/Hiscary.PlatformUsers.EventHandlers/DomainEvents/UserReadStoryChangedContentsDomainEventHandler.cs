using Hiscary.PlatformUsers.Domain.DataAccess;
using Hiscary.PlatformUsers.DomainEvents;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using Wolverine;

namespace Hiscary.PlatformUsers.EventHandlers.DomainEvents;

public sealed class UserReadStoryChangedContentsDomainEventHandler(
    IPlatformUserWriteRepository repository,
    ILogger<UserReadStoryChangedContentsDomainEventHandler> logger)
        : IEventHandler<UserReadStoryChangedContentsDomainEvent>
{
    private readonly IPlatformUserWriteRepository _repository = repository;

    public async Task Handle(
        UserReadStoryChangedContentsDomainEvent domainEvent, IMessageContext context)
    {
        var user = await _repository.GetByUserAccountId(domainEvent.UserAccountId);

        if (user is null)
        {
            return;
        }

        user.UpdateHistoryAfterStoryContentsChanges(domainEvent.StoryId, domainEvent.NumberOfPages);

        await _repository.SaveChanges();

        logger.LogInformation("{Handler} handled.", nameof(UserReadStoryChangedContentsDomainEventHandler));
    }
}
