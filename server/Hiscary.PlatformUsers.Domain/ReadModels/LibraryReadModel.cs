﻿namespace Hiscary.PlatformUsers.Domain.ReadModels;

public class LibraryReadModel
{
    public PlatformUserReadModel PlatformUser { get; set; }

    public Guid Id { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public List<string> LinksToSocialMedia { get; set; } = [];

    public bool IsLibraryOwner { get; set; }
    public bool IsSubscribed { get; set; }
    public int SubscribersCount { get; set; }

    public static LibraryReadModel FromDomainModel(Library library, Guid requesterId, bool isUserSubscribed)
    {
        return new()
        {
            PlatformUser = PlatformUserReadModel.FromDomainModel(library.PlatformUser),
            Id = library.Id,
            Bio = library.Bio,
            AvatarUrl = library.AvatarUrl,
            LinksToSocialMedia = library.LinksToSocialMedia,
            IsLibraryOwner = library.PlatformUser.UserAccountId == requesterId,
            IsSubscribed = isUserSubscribed,
            SubscribersCount = library.SubscribersCount
        };
    }
}
