using Hiscary.Media.IntegrationEvents.Outgoing;
using Hiscary.Shared.Domain.FileStorage;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventPublishers;
using StackNucleus.DDD.Domain.ResultModels;
using System.Net.Mime;

namespace Hiscary.Media.Application.Write.Services;

public sealed class MediaWriteService(
    IBlobStorageService storageService,
    IEventPublisher publisher,
    ILogger<MediaWriteService> logger) : IMediaWriteService
{
    private readonly IBlobStorageService _storageService = storageService;
    private readonly IEventPublisher _publisher = publisher;
    private readonly ILogger<MediaWriteService> _logger = logger;

    public async Task<OperationResult> UploadUserAnnotatedPdf(
        Guid userId,
        Guid storyId,
        byte[] bytes,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Attempting to upload annotated PDF for user {UserId}, story {StoryId}",
            userId, storyId);

        var blobName = BuildUserAnnotatedPdfFileName(userId, storyId);

        var link = await _storageService.UploadAsync(
            "documents",
            blobName,
            bytes,
            MediaTypeNames.Application.Pdf,
            cancellationToken);

        if (!link.HasValue || string.IsNullOrWhiteSpace(link.Value))
        {
            _logger.LogWarning(
                "Failed to upload annotated PDF for user {UserId}, story {StoryId}",
                userId, storyId);
            return OperationResult.CreateValidationsError("Failed to upload the annotated PDF.");
        }

        await _publisher.Publish(new UserAnnotatedPdfUploadedIntegrationEvent(userId, storyId, link.Value));

        _logger.LogInformation(
            "Successfully uploaded annotated PDF for user {UserId}, story {StoryId}",
            userId, storyId);
        return OperationResult.CreateSuccess();
    }

    public async Task<OperationResult> DeleteUserAnnotatedPdf(
        Guid userId,
        Guid storyId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Attempting to delete annotated PDF for user {UserId}, story {StoryId}",
            userId, storyId);

        var blobName = BuildUserAnnotatedPdfFileName(userId, storyId);

        var result = await _storageService.DeleteAsync(
            "documents",
            blobName,
            cancellationToken);

        if (!result.HasValue || !result.Value)
        {
            _logger.LogWarning(
                "Failed to delete annotated PDF for user {UserId}, story {StoryId}",
                userId, storyId);
            return OperationResult.CreateValidationsError("Failed to delete the annotated PDF.");
        }

        await _publisher.Publish(new UserAnnotatedPdfDeletedIntegrationEvent(userId, storyId));

        _logger.LogInformation(
            "Successfully deleted annotated PDF for user {UserId}, story {StoryId}",
            userId, storyId);
        return OperationResult.CreateSuccess();
    }

    private static string BuildUserAnnotatedPdfFileName(Guid userId, Guid storyId)
    {
        return $"{userId}-{storyId}.pdf";
    }
}
