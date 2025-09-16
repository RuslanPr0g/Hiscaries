using Hiscary.Shared.Domain.ValueObjects;
using StackNucleus.DDD.Domain;

namespace Hiscary.PlatformUsers.Domain;

/// <summary>
/// Represents a collection of stories published by a <see cref="PlatformUser"/>. Each story
/// is linked to a single library, and the library belongs exclusively to the 
/// <see cref="PlatformUser"/> who created it.
/// </summary>
public sealed class Library : Entity<LibraryId>
{
    public Library(
        LibraryId id,
        PlatformUserId userId) : base(id)
    {
        PlatformUserId = userId;
        SubscribersCount = 0;
    }

    public PlatformUserId PlatformUserId { get; init; }
    public PlatformUser PlatformUser { get; init; }

    public string? Bio { get; private set; }
    public ImageContainer? AvatarImageUrls { get; private set; }
    public List<string> LinksToSocialMedia { get; private set; } = [];

    public int SubscribersCount { get; set; }

    public void SubscribeUser()
    {
        SubscribersCount++;
    }

    public void UnsubscribeUser()
    {
        if (SubscribersCount > 0)
        {
            SubscribersCount--;
        }
    }

    public void Edit(string? bio, List<string> linksToSocialMedia)
    {
        Bio = bio;
        LinksToSocialMedia = linksToSocialMedia;
    }

    public void UpdateAvatarUrl(ImageContainer? avatarUrls)
    {
        AvatarImageUrls = avatarUrls;
    }

    private Library()
    {
    }
}