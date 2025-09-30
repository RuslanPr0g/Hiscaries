using StackNucleus.DDD.Domain.Events;
using StackNucleus.DDD.Domain.Images;

namespace Hiscary.Stories.IntegrationEvents.Outgoing;

public sealed class StoryUpdatedIntegrationEvent(
    Guid LibraryId,
    Guid StoryId,
    string Title,
    ImageUrlToSize[] ImageUrls,
    string Description,
    string[] Genres,
    DateTime PublishedDate,
    long UniqueReads) : BaseIntegrationEvent
{
    public Guid LibraryId { get; } = LibraryId;
    public Guid StoryId { get; } = StoryId;
    public string Title { get; } = Title;
    public ImageUrlToSize[] ImageUrls { get; set; } = ImageUrls;

    public string Description { get; } = Description;
    public string[] Genres { get; } = Genres;
    public DateTime PublishedDate { get; } = PublishedDate;
    public long UniqueReads { get; } = UniqueReads;
}
