using StackNucleus.DDD.Domain.Events;
using StackNucleus.DDD.Domain.Images.Uploaders;

namespace Hiscary.Stories.IntegrationEvents.Outgoing;

public sealed class StoryPublishedIntegrationEvent(
    Guid LibraryId,
    Guid StoryId,
    string Title,
    ImageUrlToSize[] ImageUrls) : BaseIntegrationEvent
{
    public Guid LibraryId { get; } = LibraryId;
    public Guid StoryId { get; } = StoryId;
    public string Title { get; } = Title;
    public ImageUrlToSize[] ImageUrls { get; set; } = ImageUrls;
}
