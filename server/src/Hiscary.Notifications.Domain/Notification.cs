using Hiscary.Notifications.DomainEvents;
using Hiscary.Shared.Domain.ValueObjects;
using StackNucleus.DDD.Domain;

namespace Hiscary.Notifications.Domain;

public sealed class Notification : AggregateRoot<NotificationId>
{
    private Notification(
        NotificationId id,
        Guid userId,
        string message,
        string type,
        Guid? refId = null,
        ImageContainer? imageUrls = null) : base(id)
    {
        UserId = userId;
        Message = message;
        Type = type;
        RelatedObjectId = refId;
        ImageUrls = imageUrls;

        IsRead = false;

        PublishNotificationCreatedEvent();
    }

    public static Notification CreatePublishedNotification(
        NotificationId id,
        Guid userId,
        string message,
        string type,
        Guid objectReferenceId,
        ImageContainer? imageUrls = null)
    {
        return new Notification(
            id,
            userId,
            message,
            type,
            objectReferenceId,
            imageUrls);
    }

    public Guid UserId { get; }
    public string Message { get; }
    public bool IsRead { get; private set; }
    public string Type { get; }
    public Guid? RelatedObjectId { get; }

    public ImageContainer? ImageUrls { get; private set; }

    public void UpdateImageUrls(ImageContainer container)
    {
        ImageUrls = container;
    }

    public void Read()
    {
        IsRead = true;
    }

    private void PublishNotificationCreatedEvent()
    {
        PublishEvent(new NotificationCreatedDomainEvent(Id, UserId, Type, Message));
    }

    private Notification()
    {
    }
}