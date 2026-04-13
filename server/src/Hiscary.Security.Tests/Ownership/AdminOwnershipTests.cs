using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Hiscary.Stories.Application.Write.Services;
using Hiscary.Stories.Domain.DataAccess;
using Hiscary.Stories.Domain.Stories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Hiscary.Security.Tests.Ownership;

/// <summary>
/// Property-Based Tests for StoryOwnershipValidator.IsOwnerOrAdmin() — admin short-circuit.
///
/// Validates: Requirements 2.2
///
/// Property: ∀ admin a, ∀ story s → IsOwnerOrAdmin(s, a, "admin") returns true
/// </summary>
public class AdminOwnershipTests
{
    /// <summary>
    /// Validates: Requirements 2.2
    ///
    /// Property: ∀ admin a, ∀ story s owned by a different random publisher →
    ///   IsOwnerOrAdmin(s, a, "admin") returns true
    ///   AND the story repository is NOT called (admin short-circuit)
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Admin_CanAlwaysAccessAnyStory_AndRepositoryIsNotCalled()
    {
        var adminId = Guid.NewGuid();
        var publisherId = Guid.NewGuid();
        var storyId = Guid.NewGuid();
        var libraryId = Guid.NewGuid();

        var storyRepoMock = new Mock<IStoryWriteRepository>();
        var libraryOwnerRepoMock = new Mock<ILibraryOwnerRepository>();

        // Story is owned by a different publisher — admin should bypass this entirely
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

        libraryOwnerRepoMock
            .Setup(r => r.GetOwnerUserAccountIdByLibraryId(libraryId))
            .ReturnsAsync(publisherId);

        var validator = new StoryOwnershipValidator(
            storyRepoMock.Object,
            libraryOwnerRepoMock.Object,
            NullLogger<StoryOwnershipValidator>.Instance);

        var result = validator.IsOwnerOrAdmin(storyId, adminId, "admin").GetAwaiter().GetResult();

        // Admin should always get true
        var isTrue = Prop.Label(
            result,
            $"Expected true for admin={adminId} accessing story owned by publisher={publisherId}, but got false");

        // Repository should NOT be called — admin short-circuits before any DB access
        storyRepoMock.Verify(r => r.GetById(It.IsAny<StoryId>()), Times.Never);

        return isTrue;
    }
}
