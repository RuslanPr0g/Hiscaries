using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Hiscary.Stories.Application.Write.Services;
using Hiscary.Stories.Domain.DataAccess;
using Hiscary.Stories.Domain.Stories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Hiscary.Security.IntegrationTests.Ownership;

public class StoryOwnershipValidatorTests
{
    [Property(MaxTest = 100, Arbitrary = new[] { typeof(TwoDistinctPublishersArbitrary) })]
    public Property NonOwnerPublisher_CannotAccessAnotherPublishersStory(TwoDistinctPublishers publishers)
    {
        var (p1Id, p2Id) = (publishers.P1, publishers.P2);

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

        var validator = new StoryOwnershipValidator(
            storyRepoMock.Object,
            NullLogger<StoryOwnershipValidator>.Instance);

        var result = validator.IsOwnerOrAdmin(storyId, p1Id, "publisher").GetAwaiter().GetResult();

        return Prop.Label(
            !result,
            $"Expected false for non-owner p1={p1Id} accessing story owned by p2={p2Id}, but got true");
    }

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

        var validator = new StoryOwnershipValidator(
            storyRepoMock.Object,
            NullLogger<StoryOwnershipValidator>.Instance);

        var result = validator.IsOwnerOrAdmin(storyId, p2Id, "publisher").GetAwaiter().GetResult();

        return Prop.Label(
            result,
            $"Expected true for owner p2={p2Id} accessing their own story, but got false");
    }
}

public record TwoDistinctPublishers(Guid P1, Guid P2);

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
