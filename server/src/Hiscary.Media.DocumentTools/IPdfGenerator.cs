namespace Hiscary.Media.DocumentTools;

public interface IPdfGenerator
{
    Task<byte[]> GeneratePdfFromHtmlAsync(IEnumerable<string> htmlPages, CancellationToken cancellationToken = default);
}
