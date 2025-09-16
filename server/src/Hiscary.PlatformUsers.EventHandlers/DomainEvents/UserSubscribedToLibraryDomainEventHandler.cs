using Hiscary.PlatformUsers.Domain.DataAccess;
using Hiscary.PlatformUsers.DomainEvents;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using Wolverine;

namespace Hiscary.PlatformUsers.EventHandlers.DomainEvents;

public sealed class UserSubscribedToLibraryDomainEventHandler(
    IPlatformUserWriteRepository repository,
    ILogger<UserSubscribedToLibraryDomainEventHandler> logger)
        : IEventHandler<UserSubscribedToLibraryDomainEvent>
{
    private readonly IPlatformUserWriteRepository _repository = repository;

    public async Task Handle(
        UserSubscribedToLibraryDomainEvent domainEvent, IMessageContext context)
    {
        var user = await _repository.GetLibraryOwnerByLibraryId(domainEvent.LibraryId);

        if (user is null)
        {
            return;
        }

        var library = user.GetCurrentLibrary();

        if (library is null)
        {
            return;
        }

        library.SubscribeUser();

        await _repository.SaveChanges();

        logger.LogInformation("{Handler} handled.", nameof(UserSubscribedToLibraryDomainEventHandler));
    }
}
