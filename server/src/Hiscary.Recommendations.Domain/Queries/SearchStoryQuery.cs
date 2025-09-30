using StackNucleus.DDD.Domain.ClientModels;

namespace Hiscary.Recommendations.Domain.Queries;

public sealed record SearchStoryQuery : ClientQueryableModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<string> Genres { get; set; }
    public Guid LibraryId { get; set; }
    public DateTime PublishedDate { get; set; }
}
