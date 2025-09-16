using Hiscary.Shared.Domain.ReadModels;
using Hiscary.Stories.Domain.Stories;

namespace Hiscary.Stories.Domain.ReadModels;

public sealed class StoryWithContentsReadModel : StorySimpleReadModel
{
    public IEnumerable<GenreReadModel> Genres { get; set; }
    public IEnumerable<StoryPageReadModel> Contents { get; set; }

    public StoryWithContentsReadModel() : base()
    {
    }

    public new static StoryWithContentsReadModel FromDomainModel(Story story)
    {
        return new StoryWithContentsReadModel
        {
            Id = story.Id,
            Title = story.Title,
            Description = story.Description,
            AuthorName = story.AuthorName,
            AgeLimit = story.AgeLimit,
            DatePublished = story.CreatedAt,
            DateWritten = story.DateWritten,
            LibraryId = story.LibraryId,
            Genres = story.Genres?.Select(GenreReadModel.FromDomainModel) ?? [],
            Contents = story.Contents?.Select(StoryPageReadModel.FromDomainModel) ?? [],
            ImagePreviewUrl = ImageUrlsDto.FromDomainModel(story.ImagePreviewUrl),
            TotalPages = story.TotalPages
        };
    }
}