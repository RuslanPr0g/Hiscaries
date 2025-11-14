using Hiscary.Media.DocumentTools.Models;
using UglyToad.PdfPig;

namespace Hiscary.Media.DocumentTools.PdfPig;

internal sealed class PdfDocumentTool : IDocumentTool
{
    public DocumentContent FileStreamToContent(Stream stream)
    {
        var pages = new List<DocumentPage>();

        using (var document = PdfDocument.Open(stream))
        {
            int pageNumber = 1;
            foreach (var page in document.GetPages())
            {
                pages.Add(new DocumentPage
                {
                    Page = pageNumber,
                    Text = page.Text
                });
                pageNumber++;
            }
        }

        return new DocumentContent { Pages = pages };
    }
}
