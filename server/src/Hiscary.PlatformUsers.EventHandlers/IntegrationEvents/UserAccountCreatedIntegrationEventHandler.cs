using Hiscary.PlatformUsers.Domain;
using Hiscary.PlatformUsers.Domain.DataAccess;
using Hiscary.UserAccounts.IntegrationEvents.Outgoing;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using StackNucleus.DDD.Domain.Generators;
using Wolverine;

namespace Hiscary.PlatformUsers.EventHandlers.IntegrationEvents;

public sealed class UserAccountCreatedIntegrationEventHandler(
    IPlatformUserWriteRepository repository,
    ILogger<UserAccountCreatedIntegrationEventHandler> logger,
    IIdGenerator idGenerator) : IEventHandler<UserAccountCreatedIntegrationEvent>
{
    private readonly IPlatformUserWriteRepository _repository = repository;
    private readonly IIdGenerator _idGenerator = idGenerator;

    public async Task Handle(
        UserAccountCreatedIntegrationEvent domainEvent, IMessageContext context)
    {
        var user = await _repository.GetByUserAccountId(domainEvent.UserAccountId);

        if (user is not null)
        {
            return;
        }

        var platformUserId = _idGenerator.Generate((id) => new PlatformUserId(id));
        var platformUser = new PlatformUser(platformUserId, domainEvent.UserAccountId, domainEvent.Username);

        await _repository.Add(platformUser);
        await _repository.SaveChanges();

        logger.LogInformation("{Handler} handled.", nameof(UserAccountCreatedIntegrationEventHandler));
    }
}
