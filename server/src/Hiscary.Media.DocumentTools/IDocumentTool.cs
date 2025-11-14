using Hiscary.Media.DocumentTools.Models;

namespace Hiscary.Media.DocumentTools;

public interface IDocumentTool
{
    DocumentContent FileStreamToContent(Stream stream);
}
