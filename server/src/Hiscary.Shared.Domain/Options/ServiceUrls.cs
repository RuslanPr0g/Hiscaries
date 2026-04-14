namespace Hiscary.Shared.Domain.Options;

public class ServiceUrls
{
    public string MediaServiceUrl { get; set; }

    public string GetImagesUrl(string fileName)
    {
        return $"{MediaServiceUrl}/images/{fileName}";
    }

    public string GetDocumentsUrl(string fileName)
    {
        return $"{MediaServiceUrl}/documents/{fileName}";
    }
}
