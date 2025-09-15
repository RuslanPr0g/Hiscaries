namespace Hiscary.Shared.Domain.FileStorage;

public sealed record FileWithData
{
    public string Name { get; set; }
    public byte[] Data { get; set; }
}
