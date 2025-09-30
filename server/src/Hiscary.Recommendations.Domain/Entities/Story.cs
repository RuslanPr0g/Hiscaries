namespace Hiscary.Recommendations.Domain.Entities;

public sealed class Story
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public HashSet<string> Genres { get; set; }

    public Guid LibraryId { get; set; }

    public DateTime PublishedDate { get; set; }

    public long UniqueReads { get; set; }
}
