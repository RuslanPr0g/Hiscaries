using Hiscary.Shared.Domain.ReadModels;
using Hiscary.Stories.Domain.Stories;
using StackNucleus.DDD.Domain.ReadModels;

namespace Hiscary.Stories.Domain.ReadModels;

public class StorySimpleReadModel : IReadModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string AuthorName { get; set; }
    public string[] GenreNames { get; set; }
    public int AgeLimit { get; set; }
    public ImageUrlsDto? ImagePreviewUrl { get; set; }
    public DateTime DatePublished { get; set; }
    public DateTime DateWritten { get; set; }
    public Guid LibraryId { get; set; }
    public int TotalPages { get; set; }
    public long UniqueReads { get; set; }

    public static StorySimpleReadModel FromDomainModel(Story story)
    {
        return new StorySimpleReadModel
        {
            Id = story.Id,
            Title = story.Title,
            Description = story.Description,
            AuthorName = story.AuthorName,
            AgeLimit = story.AgeLimit,
            DatePublished = story.CreatedAt,
            DateWritten = story.DateWritten,
            LibraryId = story.LibraryId,
            ImagePreviewUrl = ImageUrlsDto.FromDomainModel(story.ImagePreviewUrl),
            TotalPages = story.TotalPages,
            UniqueReads = story.UniqueReads,
            GenreNames = story.Genres?.Select(_ => _.Name).ToArray() ?? []
        };
    }
}