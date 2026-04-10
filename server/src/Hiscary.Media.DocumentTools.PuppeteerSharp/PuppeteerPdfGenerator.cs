using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace Hiscary.Media.DocumentTools.PuppeteerSharp;

internal sealed class PuppeteerPdfGenerator : IPdfGenerator
{
    private static readonly SemaphoreSlim _semaphore = new(1, 1);
    private static IBrowser? _browser;

    private static async Task<IBrowser> GetBrowserAsync()
    {
        if (_browser is not null)
        {
            return _browser;
        }

        await _semaphore.WaitAsync();
        try
        {
            if (_browser is not null)
            {
                return _browser;
            }

            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            _browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
            });

            return _browser;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<byte[]> GeneratePdfFromHtmlAsync(IEnumerable<string> htmlPages, CancellationToken cancellationToken = default)
    {
        var browser = await GetBrowserAsync();

        using var page = await browser.NewPageAsync();

        var htmlPagesList = htmlPages.ToList();
        var htmlWithPageBreaks = string.Join("", htmlPagesList.Select((html, index) =>
        {
            var pageBreakStyle = index < htmlPagesList.Count - 1 ? "style='page-break-after: always;'" : "";
            return $"<div {pageBreakStyle}>{html}</div>";
        }));

        await page.SetContentAsync(htmlWithPageBreaks);

        var pdfBytes = await page.PdfDataAsync(new PdfOptions
        {
            Format = PaperFormat.A4,
            PrintBackground = true,
            MarginOptions = new MarginOptions
            {
                Top = "10px",
                Bottom = "10px",
                Left = "10px",
                Right = "10px"
            }
        });

        await page.CloseAsync();

        return pdfBytes;
    }
}
