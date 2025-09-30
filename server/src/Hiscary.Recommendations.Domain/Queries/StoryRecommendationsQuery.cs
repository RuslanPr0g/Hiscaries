using StackNucleus.DDD.Domain.ClientModels;

namespace Hiscary.Recommendations.Domain.Queries;

public sealed record StoryRecommendationsQuery : ClientQueryableModel
{
    public Guid UserAccountId { get; set; }
}
