namespace Hiscary.Media.DocumentTools.Models;

public sealed record DocumentPage
{
    public int Page { get; init; }
    public string Text { get; init; }
}
