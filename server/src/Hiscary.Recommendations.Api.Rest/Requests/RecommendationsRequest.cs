using Hiscary.Recommendations.Domain.Queries;

namespace Hiscary.Recommendations.Api.Rest.Requests;

public class RecommendationsRequest
{
    public required int StartIndex { get; set; }
    public required int ItemsCount { get; set; }
}
