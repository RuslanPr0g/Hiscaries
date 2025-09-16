using StackNucleus.DDD.Domain.EventHandlers;
using Hiscary.PlatformUsers.Domain.DataAccess;
using Hiscary.PlatformUsers.DomainEvents;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace Hiscary.PlatformUsers.EventHandlers.DomainEvents;

public sealed class UserUnsubscribedFromLibraryDomainEventHandler(
    IPlatformUserWriteRepository repository,
    ILogger<UserUnsubscribedFromLibraryDomainEventHandler> logger)
        : IEventHandler<UserUnsubscribedFromLibraryDomainEvent>
{
    private readonly IPlatformUserWriteRepository _repository = repository;

    public async Task Handle(
        UserUnsubscribedFromLibraryDomainEvent domainEvent, IMessageContext context)
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

        library.UnsubscribeUser();

        await _repository.SaveChanges();

        logger.LogInformation("{Handler} handled.", nameof(UserUnsubscribedFromLibraryDomainEventHandler));
    }
}
