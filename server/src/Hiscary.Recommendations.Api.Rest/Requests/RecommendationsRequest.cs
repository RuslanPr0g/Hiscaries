using Hiscary.Recommendations.Domain.Queries;

namespace Hiscary.Recommendations.Api.Rest.Requests;

public class RecommendationsRequest
{
    public StoryRecommendationsQuery QueryableModel { get; set; }
}
