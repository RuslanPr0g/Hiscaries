using StackNucleus.DDD.Domain.Events;

namespace Hiscary.Media.IntegrationEvents.Outgoing;

public sealed class UserAnnotatedPdfUploadedIntegrationEvent(
    Guid UserAccountId,
    Guid StoryId,
    string PdfUrl
) : BaseIntegrationEvent
{
    public Guid UserAccountId { get; set; } = UserAccountId;
    public Guid StoryId { get; set; } = StoryId;
    public string PdfUrl { get; set; } = PdfUrl;
}
