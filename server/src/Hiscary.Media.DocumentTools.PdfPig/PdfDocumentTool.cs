using Hiscary.Media.DocumentTools;
using Hiscary.Media.DocumentTools.Models;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

internal sealed class PdfDocumentTool : IDocumentTool
{
    private static readonly HashSet<string> HeaderKeywordsEn = new() { "Contents", "Chapter", "Section", "Part" };
    private static readonly HashSet<string> HeaderKeywordsUa = new() { "Зміст", "Розділ", "ЧАСТИНА" };

    public DocumentContent FileStreamToContent(Stream stream, int? start, int? end)
    {
        var pages = new List<DocumentPage>();

        using var document = PdfDocument.Open(stream);
        var allPages = document.GetPages().ToList();
        int totalPages = allPages.Count;

        int s = Math.Max(start.GetValueOrDefault(1), 1);
        int e = Math.Min(end.GetValueOrDefault(totalPages), totalPages);
        int pageNum = s;

        for (int i = s - 1; i < e; i++)
        {
            var page = allPages[i];
            var htmlText = BuildHtmlFromPage(page);

            if (string.IsNullOrWhiteSpace(htmlText))
                continue;

            pages.Add(new DocumentPage
            {
                Page = pageNum++,
                Text = htmlText
            });
        }

        return new DocumentContent { Pages = pages };
    }

    private static string BuildHtmlFromPage(Page page)
    {
        var words = page.GetWords().ToList();
        if (!words.Any())
            return string.Empty;

        var sb = new StringBuilder();
        double lastRight = words[0].BoundingBox.Right;
        double lastBottom = words[0].BoundingBox.Bottom;

        var lineWords = new List<Word>();

        foreach (var word in words)
        {
            if (Math.Abs(word.BoundingBox.Bottom - lastBottom) > 2)
            {
                AppendLine(sb, lineWords);
                lineWords.Clear();
            }
            lineWords.Add(word);
            lastRight = word.BoundingBox.Right;
            lastBottom = word.BoundingBox.Bottom;
        }
        AppendLine(sb, lineWords);

        return sb.ToString();
    }

    private static void AppendLine(StringBuilder sb, List<Word> lineWords)
    {
        if (!lineWords.Any())
            return;

        string lineText = string.Join(" ", lineWords.Select(w => w.Text));

        bool isHeader = IsHeader(lineText);

        if (isHeader)
        {
            sb.AppendLine($"<h2>{lineText}</h2>");
        }
        else
        {
            sb.AppendLine($"<p>{lineText}</p>");
        }
    }

    private static bool IsHeader(string text)
    {
        string t = text.Trim();
        if (string.IsNullOrEmpty(t))
            return false;

        if (HeaderKeywordsEn.Any(k => t.Contains(k, StringComparison.OrdinalIgnoreCase)))
            return true;

        if (HeaderKeywordsUa.Any(k => t.Contains(k, StringComparison.OrdinalIgnoreCase)))
            return true;

        if (t.All(c => char.IsUpper(c) || char.IsWhiteSpace(c) || char.IsDigit(c)))
            return true;

        return false;
    }
}
