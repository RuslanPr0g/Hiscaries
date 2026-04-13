using Hiscary.Stories.Domain.DataAccess;
using Hiscary.Stories.Domain.Stories;
using Microsoft.Extensions.Logging;

namespace Hiscary.Stories.Application.Write.Services;

public sealed class StoryOwnershipValidator(
    IStoryWriteRepository storyRepository,
    ILogger<StoryOwnershipValidator> logger) : IStoryOwnershipValidator
{
    private const string AdminRole = "admin";

    public async Task<bool> IsOwnerOrAdmin(Guid storyId, Guid callerLibraryId, string callerRole)
    {
        if (callerRole == AdminRole)
        {
            return true;
        }

        var story = await storyRepository.GetById(storyId);

        if (story is null)
        {
            logger.LogWarning("Story {StoryId} not found during ownership check", storyId);
            return false;
        }

        return story.LibraryId == callerLibraryId;
    }

    public async Task<bool> IsCommentOwner(Guid storyId, Guid commentId, Guid callerPlatformUserId, string callerRole)
    {
        if (callerRole == AdminRole)
        {
            return true;
        }

        var story = await storyRepository.GetById(storyId);

        if (story is null)
        {
            logger.LogWarning("Story {StoryId} not found during comment ownership check", storyId);
            return false;
        }

        var comment = story.Comments.FirstOrDefault(c => c.Id == new CommentId(commentId));

        if (comment is null)
        {
            logger.LogWarning("Comment {CommentId} not found in story {StoryId} during ownership check", commentId, storyId);
            return false;
        }

        return comment.PlatformUserId == callerPlatformUserId;
    }
}
