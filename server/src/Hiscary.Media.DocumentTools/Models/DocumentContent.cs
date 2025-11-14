namespace Hiscary.Media.DocumentTools.Models;

public sealed record DocumentContent
{
    public List<DocumentPage> Pages { get; init; } = [];
}
