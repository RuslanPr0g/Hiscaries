namespace Hiscary.Recommendations.Domain.Entities;

public sealed record Story
{
    public Guid Id { get; init; }

    public string Title { get; init; }

    public string Description { get; init; }

    public HashSet<string> Genres { get; init; }

    public Guid LibraryId { get; init; }

    public DateTime PublishedDate { get; init; }

    public long UniqueReads { get; init; }
}
