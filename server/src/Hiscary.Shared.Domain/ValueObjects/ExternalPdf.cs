namespace Hiscary.Shared.Domain.ValueObjects;

public sealed class ExternalPdf : IEquatable<ExternalPdf>
{
    private ExternalPdf()
    {
        PdfUrl = string.Empty;
        FileSize = 0;
    }

    public ExternalPdf(string pdfUrl, long fileSize)
    {
        PdfUrl = pdfUrl;
        FileSize = fileSize;
    }

    public static ExternalPdf FromUrlAndSize(string pdfUrl, long fileSize)
    {
        if (string.IsNullOrWhiteSpace(pdfUrl))
            throw new ArgumentException("PDF URL cannot be empty.", nameof(pdfUrl));

        if (fileSize < 0)
            throw new ArgumentException("File size cannot be negative.", nameof(fileSize));

        return new ExternalPdf(pdfUrl, fileSize);
    }

    public string PdfUrl { get; init; } = string.Empty;
    public long FileSize { get; init; }

    public bool Equals(ExternalPdf? other)
    {
        if (other is null) return false;
        return PdfUrl == other.PdfUrl && FileSize == other.FileSize;
    }

    public override bool Equals(object? obj) => Equals(obj as ExternalPdf);

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (PdfUrl?.GetHashCode() ?? 0);
            hash = hash * 23 + FileSize.GetHashCode();
            return hash;
        }
    }
}
