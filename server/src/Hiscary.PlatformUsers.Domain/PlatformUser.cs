using Hiscary.PlatformUsers.DomainEvents;
using Hiscary.Shared.Domain.ValueObjects;
using StackNucleus.DDD.Domain;

namespace Hiscary.PlatformUsers.Domain;

/// <summary>
/// Represents a general user of the platform who can perform actions such as 
/// reading, commenting on stories, and optionally creating and managing one or 
/// more libraries. Each published story is linked to a single library owned by 
/// this user.
/// </summary>
public sealed class PlatformUser : AggregateRoot<PlatformUserId>
{
    public PlatformUser(
        PlatformUserId id,
        Guid userId,
        string username) : base(id)
    {
        UserAccountId = userId;
        Username = username;
    }

    public Guid UserAccountId { get; init; }

    public ICollection<Review> Reviews { get; } = [];
    public ICollection<ReadingHistory> ReadHistory { get; } = [];
    public ICollection<StoryBookMark> Bookmarks { get; } = [];

    public ICollection<PlatformUserToLibrarySubscription> Subscriptions { get; } = [];

    public bool IsPublisher => Libraries.Count > 0;

    // For now, we allow only one library to be added, but in the future, we can allow to create multiple libraries
    public ICollection<Library> Libraries { get; } = [];

    // TODO: this value should be synced with the user account, as it should be the same (use domain events)
    public string Username { get; set; }

    public Library? GetCurrentLibrary()
    {
        if (!IsPublisher)
        {
            return null;
        }

        return Libraries.ElementAt(0);
    }

    public void SubscribeToLibrary(LibraryId libraryId)
    {
        if (Libraries.Any(x => x.Id == libraryId))
        {
            return;
        }

        var subscription = Subscriptions.FirstOrDefault(x => x.LibraryId == libraryId);

        if (subscription is null)
        {
            Subscriptions.Add(new PlatformUserToLibrarySubscription(Id, libraryId));
            PublishSubscribedToLibrary(libraryId);
        }
    }

    public void UnsubscribeFromLibrary(LibraryId libraryId)
    {
        var subscription = Subscriptions.FirstOrDefault(x => x.LibraryId == libraryId);

        if (subscription is not null)
        {
            Subscriptions.Remove(subscription);
            PublishUnsubscribedFromLibrary(libraryId);
        }
    }

    public void BecomePublisher(LibraryId libraryId)
    {
        if (!IsPublisher)
        {
            Libraries.Add(new Library(libraryId, Id));
        }
    }

    public void ReadStoryPage(Guid storyId, int page)
    {
        var historyItem = ReadHistory.FirstOrDefault(x => x.StoryId == storyId);

        if (historyItem is not null)
        {
            historyItem.ReadPage(page);
        }
        else
        {
            ReadHistory.Add(new ReadingHistory(Id, storyId, page));
        }
    }

    public void RemoveReview(LibraryId libraryId)
    {
        if (Reviews.Count > 0)
        {
            var review = Reviews.FirstOrDefault(x => x.LibraryId == libraryId);

            if (review is not null)
            {
                Reviews.Remove(review);
            }
        }
    }

    public void PublishReview(LibraryId libraryId, string? message)
    {
        if (!string.IsNullOrEmpty(message) && Reviews.Count > 0)
        {
            var review = Reviews.FirstOrDefault(x => x.LibraryId == libraryId);

            if (review is null)
            {
                Reviews.Add(
                    new Review(Id, libraryId, message));
            }
            else
            {
                review.Edit(message);
            }
        }
    }

    public void EditLibrary(
        LibraryId libraryId,
        string? bio,
        List<string> linkToSocialMedia)
    {
        var library = GetCurrentLibrary();

        if (library is not null && library.Id == libraryId)
        {
            library.Edit(bio, linkToSocialMedia);
        }
    }

    public void UpdateAvatarUrl(
        LibraryId libraryId,
        ImageContainer? avatarUrls)
    {
        var library = GetCurrentLibrary();

        if (library is not null && library.Id == libraryId)
        {
            library.UpdateAvatarUrl(avatarUrls);
        }
    }

    public void ClearAvatarUrl(LibraryId libraryId)
    {
        UpdateAvatarUrl(libraryId, null);
    }

    public void BookmarkStory(Guid storyId)
    {
        if (!Bookmarks.Any(x => x.StoryId == storyId))
        {
            Bookmarks.Add(new StoryBookMark(Id, storyId));
        }
    }

    private void PublishSubscribedToLibrary(LibraryId libraryId)
    {
        PublishEvent(new UserSubscribedToLibraryDomainEvent(UserAccountId, libraryId));
    }

    private void PublishUnsubscribedFromLibrary(LibraryId libraryId)
    {
        PublishEvent(new UserUnsubscribedFromLibraryDomainEvent(UserAccountId, libraryId));
    }

    private PlatformUser()
    {
    }
}