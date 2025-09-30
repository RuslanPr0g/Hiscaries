namespace Hiscary.Recommendations.Persistence.Shared;

public class ElasticsearchConfiguration
{
    public string StoryIndex { get; set; } = "recommendations_stories";
    public string UserPreferencesIndex { get; set; } = "recommendations_preferences";
}
