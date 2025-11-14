using Hiscary.Media.DocumentTools;
using Hiscary.Media.DocumentTools.Models;
using System.Text;
using UglyToad.PdfPig;

internal sealed class PdfDocumentTool : IDocumentTool
{
    public DocumentContent FileStreamToContent(Stream stream, int? start, int? end)
    {
        var pages = new List<DocumentPage>();

        using var document = PdfDocument.Open(stream);
        var allPages = document.GetPages().ToList();
        int totalPages = allPages.Count;

        int s = Math.Max(start.GetValueOrDefault(1), 1);
        int e = Math.Min(end.GetValueOrDefault(totalPages), totalPages);
        int pageNum = s - 1;

        for (int i = s - 1; i < e; i++)
        {
            var page = allPages[i];

            var cleanText = ReconstructPageText(page);

            if (string.IsNullOrWhiteSpace(cleanText))
            {
                continue;
            }

            pages.Add(new DocumentPage
            {
                Page = pageNum++,
                Text = cleanText
            });
        }

        return new DocumentContent { Pages = pages };
    }

    private static string ReconstructPageText(UglyToad.PdfPig.Content.Page page)
    {
        var words = page.GetWords().ToList();

        if (!words.Any())
            return string.Empty;

        var sb = new StringBuilder();
        double lastRight = words[0].BoundingBox.Right;
        double lastBottom = words[0].BoundingBox.Bottom;

        foreach (var word in words)
        {
            if (Math.Abs(word.BoundingBox.Bottom - lastBottom) > 2)
            {
                sb.Append('\n');
            }
            else if (word.BoundingBox.Left > lastRight + 1) 
            {
                sb.Append(' ');
            }

            sb.Append(word.Text);

            lastRight = word.BoundingBox.Right;
            lastBottom = word.BoundingBox.Bottom;
        }

        return sb.ToString();
    }
}
