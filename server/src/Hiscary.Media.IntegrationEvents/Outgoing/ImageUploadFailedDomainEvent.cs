using StackNucleus.DDD.Domain.Events;

namespace Hiscary.Media.IntegrationEvents.Outgoing;

public sealed class ImageUploadFailedDomainEvent(
    Guid RequesterId,
    string Error
    ) : BaseIntegrationEvent
{
    public Guid RequesterId { get; set; } = RequesterId;
    // TODO: maybe error type smth too?
    public string Error { get; set; } = Error;
}
