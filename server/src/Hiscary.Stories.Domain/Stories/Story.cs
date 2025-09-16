using Hiscary.Shared.Domain.ValueObjects;
using Hiscary.Stories.Domain.Genres;
using StackNucleus.DDD.Domain;

namespace Hiscary.Stories.Domain.Stories;

public sealed class Story : AggregateRoot<StoryId>
{
    private Story(
        StoryId id,
        Guid libraryId,
        string title,
        string description,
        string authorName,
        ICollection<Genre> genres,
        int ageLimit,
        DateTime dateWritten) : base(id)
    {
        Id = id;
        LibraryId = libraryId;
        Title = title;
        Description = description;
        AuthorName = authorName;
        Genres = genres;
        AgeLimit = ageLimit;
        DateWritten = dateWritten;
    }

    public static Story Create(
        StoryId id,
        Guid libraryId,
        string title,
        string description,
        string authorName,
        ICollection<Genre> genres,
        int ageLimit,
        DateTime dateWritten) => new(
            id,
            libraryId,
            title,
            description,
            authorName,
            genres,
            ageLimit,
            dateWritten);

    public Guid LibraryId { get; init; }

    public ICollection<Genre> Genres { get; init; } = [];
    public List<StoryPage> Contents { get; private set; } = [];
    public ICollection<Comment> Comments { get; init; } = [];
    public ICollection<StoryAudio> Audios { get; init; } = [];
    public ICollection<StoryRating> Ratings { get; init; } = [];

    public string Title { get; private set; }
    public string Description { get; private set; }
    public string AuthorName { get; private set; }
    public int AgeLimit { get; private set; }
    public DateTime DateWritten { get; private set; }

    public ImageContainer? ImagePreviewUrl { get; private set; }

    public int TotalPages => Contents.Count;

    public void UpdatePreviewUrl(
        ImageContainer? previewUrl)
    {
        ImagePreviewUrl = previewUrl;
    }

    public void ClearPreviewUrl()
    {
        UpdatePreviewUrl(null);
    }

    public void UpdateTitle(string title)
    {
        if (!string.IsNullOrEmpty(title))
        {
            Title = title;
        }
    }

    public void UpdateAgeLimit(int value)
    {
        AgeLimit = value;
    }

    public void AddComment(CommentId commentId, Guid userId, string content, int score)
    {
        Comments.Add(Comment.Create(commentId, Id, userId, content, score));
    }

    public void SetScoreByUser(Guid userId, int score)
    {
        var existingRating = Ratings.FirstOrDefault(x => x.PlatformUserId == userId);

        if (existingRating is not null)
        {
            existingRating.UpdateScore(score);
        }
        else
        {
            Ratings.Add(new StoryRating(Id, userId, score));
        }
    }

    public void DeleteComment(CommentId commentId)
    {
        var comment = Comments.FirstOrDefault(x => x.Id == commentId);

        if (comment is not null)
        {
            Comments.Remove(comment);
        }
    }

    public void UpdateComment(CommentId commentId, string content, int score)
    {
        var comment = Comments.FirstOrDefault(x => x.Id == commentId);

        if (comment is not null)
        {
            comment.UpdateContent(content, score);
        }
    }

    public void UpdateInformation(
        string title,
        string description,
        string authorName,
        ICollection<Genre> genres,
        int ageLimit,
        DateTime dateWritten)
    {
        Title = title;
        Description = description;
        AuthorName = authorName;
        AgeLimit = ageLimit;
        DateWritten = dateWritten;

        Genres.Clear();

        foreach (var genre in genres)
        {
            Genres.Add(genre);
        }
    }

    public void ModifyContents(IEnumerable<string> newContents)
    {
        if (Contents is null || Contents.Count == 0)
        {
            Contents = [.. newContents.Select((content, index) => new StoryPage(Id, index, content))];
            return;
        }

        int currentIndex = 0;

        foreach (var newContent in newContents)
        {
            if (currentIndex < Contents.Count)
            {
                var existingPage = Contents[currentIndex];

                if (existingPage.Content != newContent)
                {
                    Contents[currentIndex] = new StoryPage(Id, currentIndex, newContent);
                }
            }
            else
            {
                Contents.Add(new StoryPage(Id, currentIndex, newContent));
            }

            currentIndex++;
        }

        if (currentIndex < Contents.Count)
        {
            Contents.RemoveRange(currentIndex, Contents.Count - currentIndex);
        }
    }

    public Guid? ClearAllAudio()
    {
        var fileId = Audios.FirstOrDefault()?.FileId;
        Audios.Clear();
        return fileId;
    }

    public void SetAudio(
        StoryAudioId storyAudioId,
        Guid fileId,
        string name)
    {
        var audio = Audios.FirstOrDefault();

        if (audio is not null)
        {
            audio.UpdateInformation(fileId, name);
        }
        else
        {
            Audios.Add(StoryAudio.Create(storyAudioId, name));
        }
    }

    private Story()
    {
    }
}