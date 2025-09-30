namespace Hiscary.Stories.IntegrationEvents.Outgoing.DTOs;

public sealed class StoryChunk
{
    public Guid Id { get; init; }

    public string Title { get; init; }

    public string Description { get; init; }

    public string[] Genres { get; init; }

    public Guid LibraryId { get; init; }

    public DateTime PublishedDate { get; init; }

    public long UniqueReads { get; init; }
}
