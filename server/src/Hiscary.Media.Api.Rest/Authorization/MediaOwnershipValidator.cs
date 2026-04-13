using Hiscary.Shared.Domain.Options;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Hiscary.Media.Api.Rest.Authorization;

public sealed class MediaOwnershipValidator(
    IHttpClientFactory httpClientFactory,
    ServiceUrls serviceUrls,
    ILogger<MediaOwnershipValidator> logger) : IMediaOwnershipValidator
{
    private const string AdminRole = "admin";

    public async Task<bool> IsStoryOwnerOrAdmin(Guid storyId, Guid callerId, string callerRole)
    {
        if (callerRole == AdminRole)
        {
            return true;
        }

        try
        {
            var client = httpClientFactory.CreateClient("StoriesService");
            var response = await client.PostAsJsonAsync(
                $"{serviceUrls.StoriesServiceBaseUrl}/api/v1/stories/by-id-with-contents",
                new { Id = storyId });

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Stories service returned {StatusCode} when resolving ownership for story {StoryId}",
                    response.StatusCode, storyId);
                return false;
            }

            var story = await response.Content.ReadFromJsonAsync<StoryOwnershipDto>();

            if (story is null)
            {
                logger.LogWarning("Stories service returned null response for story {StoryId}", storyId);
                return false;
            }

            return story.PublisherId == callerId;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Failed to reach Stories service when validating ownership for story {StoryId}. Denying access (fail-closed).",
                storyId);
            return false;
        }
    }

    private sealed class StoryOwnershipDto
    {
        public Guid PublisherId { get; set; }
    }
}
