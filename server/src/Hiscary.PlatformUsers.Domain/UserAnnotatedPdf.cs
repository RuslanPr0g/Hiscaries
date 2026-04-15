using StackNucleus.DDD.Domain;

namespace Hiscary.PlatformUsers.Domain;

public sealed class UserAnnotatedPdf : Entity
{
    public UserAnnotatedPdf(
        PlatformUserId platformUserId,
        Guid storyId,
        string pdfUrl)
    {
        PlatformUserId = platformUserId;
        StoryId = storyId;
        PdfUrl = pdfUrl;
        HasConflict = false;
    }

    public PlatformUserId PlatformUserId { get; init; }
    public Guid StoryId { get; init; }
    public string PdfUrl { get; set; }
    public bool HasConflict { get; set; }

    private UserAnnotatedPdf()
    {
    }
}
