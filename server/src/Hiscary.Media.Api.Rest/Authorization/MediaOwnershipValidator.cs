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
            var response = await client.GetAsync(
                $"{serviceUrls.StoriesServiceBaseUrl}/api/v1/stories/owner?storyId={storyId}");

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Stories service returned {StatusCode} when resolving ownership for story {StoryId}. Denying access (fail-closed).",
                    response.StatusCode, storyId);
                return false;
            }

            var ownerUserAccountId = await response.Content.ReadFromJsonAsync<Guid>();

            return ownerUserAccountId == callerId;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Failed to reach Stories service when validating ownership for story {StoryId}. Denying access (fail-closed).",
                storyId);
            return false;
        }
    }
}
