using Hiscary.Shared.Domain.Options;

namespace Hiscary.Media.Api.Rest.Authorization;

public sealed class MediaOwnershipValidator(
    IHttpClientFactory httpClientFactory,
    ServiceUrls serviceUrls,
    ILogger<MediaOwnershipValidator> logger) : IMediaOwnershipValidator
{
    private const string AdminRole = "admin";

    public async Task<bool> IsStoryOwnerOrAdmin(Guid storyId, Guid callerId, string callerRole, string token, CancellationToken cancellationToken = default)
    {
        if (callerRole == AdminRole)
        {
            return true;
        }

        try
        {
            var client = httpClientFactory.CreateClient("StoriesService");
            var requestMessage = new HttpRequestMessage(
                HttpMethod.Get,
                $"{serviceUrls.StoriesServiceBaseUrl}/api/v1/stories/owner?storyId={storyId}");

            if (!string.IsNullOrWhiteSpace(token))
            {
                requestMessage.Headers.TryAddWithoutValidation("Authorization", token);
            }

            var response = await client.SendAsync(requestMessage, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Stories service returned {StatusCode} when resolving ownership for story {StoryId}. Denying access (fail-closed).",
                    response.StatusCode, storyId);
                return false;
            }

            var ownerUserAccountId = await response.Content.ReadFromJsonAsync<Guid>(cancellationToken);

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
