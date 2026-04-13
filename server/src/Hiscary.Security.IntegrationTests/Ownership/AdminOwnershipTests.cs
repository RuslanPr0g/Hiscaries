using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Hiscary.Stories.Application.Write.Services;
using Hiscary.Stories.Domain.DataAccess;
using Hiscary.Stories.Domain.Stories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Hiscary.Security.IntegrationTests.Ownership;

public class AdminOwnershipTests
{
    [Property(MaxTest = 100)]
    public Property Admin_CanAlwaysAccessAnyStory_AndRepositoryIsNotCalled()
    {
        var adminId = Guid.NewGuid();
        var publisherId = Guid.NewGuid();
        var storyId = Guid.NewGuid();
        var libraryId = Guid.NewGuid();

        var storyRepoMock = new Mock<IStoryWriteRepository>();

        storyRepoMock
            .Setup(r => r.GetById(It.IsAny<StoryId>()))
            .ReturnsAsync(Story.Create(
                new StoryId(storyId),
                libraryId,
                "Test Story",
                "Description",
                "Author",
                [],
                18,
                DateTime.UtcNow));

        var validator = new StoryOwnershipValidator(
            storyRepoMock.Object,
            NullLogger<StoryOwnershipValidator>.Instance);

        var result = validator.IsOwnerOrAdmin(storyId, adminId, "admin").GetAwaiter().GetResult();

        var isTrue = Prop.Label(
            result,
            $"Expected true for admin={adminId} accessing story owned by publisher={publisherId}, but got false");

        storyRepoMock.Verify(r => r.GetById(It.IsAny<StoryId>()), Times.Never);

        return isTrue;
    }
}
