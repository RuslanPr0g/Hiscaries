namespace Hiscary.PlatformUsers.Domain.ReadModels;

public sealed record LastReadAtDateToId
{
    public Guid Id { get; set; }
    public DateTime LastReadAt { get; set; }

    public static LastReadAtDateToId Create(Guid Id, DateTime lastReadAt)
    {
        return new LastReadAtDateToId
        {
            Id = Id,
            LastReadAt = lastReadAt
        };
    }
}
