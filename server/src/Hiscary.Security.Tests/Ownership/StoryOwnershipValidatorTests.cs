using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Hiscary.Stories.Application.Write.Services;
using Hiscary.Stories.Domain.DataAccess;
using Hiscary.Stories.Domain.Genres;
using Hiscary.Stories.Domain.Stories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Hiscary.Security.Tests.Ownership;

/// <summary>
/// Property-Based Tests for StoryOwnershipValidator.IsOwnerOrAdmin().
///
/// Validates: Requirements 2.2
///
/// Property: ∀ publisher p1, story s owned by p2 (p1 ≠ p2) → IsOwnerOrAdmin(s, p1, "publisher") = false
/// Inverse:  ∀ publisher p2, story s owned by p2 → IsOwnerOrAdmin(s, p2, "publisher") = true
/// </summary>
public class StoryOwnershipValidatorTests
{
    /// <summary>
    /// Validates: Requirements 2.2
    ///
    /// Property: ∀ publisher p1, story s owned by p2 (p1 ≠ p2) →
    ///   IsOwnerOrAdmin(s, p1, "publisher") returns false
    /// </summary>
    [Property(MaxTest = 100, Arbitrary = new[] { typeof(TwoDistinctPublishersArbitrary) })]
    public Property NonOwnerPublisher_CannotAccessAnotherPublishersStory(TwoDistinctPublishers publishers)
    {
        var (p1Id, p2Id) = (publishers.P1, publishers.P2);

        var storyId = Guid.NewGuid();
        var libraryId = Guid.NewGuid();

        // Story is owned by p2's library
        var story = Story.Create(
            new StoryId(storyId),
            libraryId,
            "Test Story",
            "Description",
            "Author",
            [],
            18,
            DateTime.UtcNow);

        var storyRepoMock = new Mock<IStoryWriteRepository>();
        storyRepoMock
            .Setup(r => r.GetById(new StoryId(storyId)))
            .ReturnsAsync(story);

        var libraryOwnerRepoMock = new Mock<ILibraryOwnerRepository>();
        libraryOwnerRepoMock
            .Setup(r => r.GetOwnerUserAccountIdByLibraryId(libraryId))
            .ReturnsAsync(p2Id); // library belongs to p2

        var validator = new StoryOwnershipValidator(
            storyRepoMock.Object,
            libraryOwnerRepoMock.Object,
            NullLogger<StoryOwnershipValidator>.Instance);

        // p1 (non-owner) tries to access p2's story
        var result = validator.IsOwnerOrAdmin(storyId, p1Id, "publisher").GetAwaiter().GetResult();

        return Prop.Label(
            !result,
            $"Expected false for non-owner p1={p1Id} accessing story owned by p2={p2Id}, but got true");
    }

    /// <summary>
    /// Validates: Requirements 2.2
    ///
    /// Inverse property: ∀ publisher p2, story s owned by p2 →
    ///   IsOwnerOrAdmin(s, p2, "publisher") returns true
    /// </summary>
    [Property(MaxTest = 100, Arbitrary = new[] { typeof(TwoDistinctPublishersArbitrary) })]
    public Property OwnerPublisher_CanAccessOwnStory(TwoDistinctPublishers publishers)
    {
        var p2Id = publishers.P2;

        var storyId = Guid.NewGuid();
        var libraryId = Guid.NewGuid();

        var story = Story.Create(
            new StoryId(storyId),
            libraryId,
            "Test Story",
            "Description",
            "Author",
            [],
            18,
            DateTime.UtcNow);

        var storyRepoMock = new Mock<IStoryWriteRepository>();
        storyRepoMock
            .Setup(r => r.GetById(new StoryId(storyId)))
            .ReturnsAsync(story);

        var libraryOwnerRepoMock = new Mock<ILibraryOwnerRepository>();
        libraryOwnerRepoMock
            .Setup(r => r.GetOwnerUserAccountIdByLibraryId(libraryId))
            .ReturnsAsync(p2Id); // library belongs to p2

        var validator = new StoryOwnershipValidator(
            storyRepoMock.Object,
            libraryOwnerRepoMock.Object,
            NullLogger<StoryOwnershipValidator>.Instance);

        // p2 (owner) accesses their own story
        var result = validator.IsOwnerOrAdmin(storyId, p2Id, "publisher").GetAwaiter().GetResult();

        return Prop.Label(
            result,
            $"Expected true for owner p2={p2Id} accessing their own story, but got false");
    }
}

/// <summary>
/// Represents two distinct publisher GUIDs (p1 ≠ p2).
/// </summary>
public record TwoDistinctPublishers(Guid P1, Guid P2);

/// <summary>
/// FsCheck arbitrary that generates pairs of distinct non-empty GUIDs
/// representing two different publishers.
/// </summary>
public static class TwoDistinctPublishersArbitrary
{
    public static Arbitrary<TwoDistinctPublishers> Generate()
    {
        var gen =
            from p1 in Gen.Fresh(Guid.NewGuid)
            from p2 in Gen.Fresh(Guid.NewGuid).Where(g => g != p1)
            select new TwoDistinctPublishers(p1, p2);

        return gen.ToArbitrary();
    }
}
