using Hiscary.Media.IntegrationEvents.Incoming;
using Hiscary.Media.IntegrationEvents.Outgoing;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventHandlers;
using StackNucleus.DDD.Domain.EventPublishers;
using StackNucleus.DDD.Domain.Images;
using StackNucleus.DDD.Domain.Images.Uploaders;
using StackNucleus.DDD.Domain.ResultModels;
using Wolverine;

namespace Hiscary.Media.EventHandlers.IntegrationEvents;

public sealed class ImageUploadRequestedIntegrationEventHandler(
    IEventPublisher publisher,
    IImageUploader imageUploader,
    ILogger<ImageUploadRequestedIntegrationEventHandler> logger)
    : IEventHandler<ImageUploadRequestedIntegrationEvent>
{
    private readonly IImageUploader _imageUploader = imageUploader;
    private readonly IEventPublisher _publisher = publisher;
    private readonly ILogger<ImageUploadRequestedIntegrationEventHandler> _logger = logger;

    public async Task Handle(
        ImageUploadRequestedIntegrationEvent integrationEvent, IMessageContext context)
    {
        var file = integrationEvent.Content;
        var requesterId = integrationEvent.RequesterId;
        var sizes = integrationEvent.Sizes;

        try
        {
            var result = await UploadFileAsync(requesterId, file, sizes);

            if (!result.IsSuccess || !result.HasValue || result.Value is null)
            {
                await PublishFail(requesterId, result.Error ?? "Unexpected error occured.");
                return;
            }

            if (result.Value.Images.All(_ => string.IsNullOrWhiteSpace(_.Url)))
            {
                await PublishFail(requesterId, "Could not generate URL. No details can be provided.");
            }
            else
            {
                await PublishSuccess(requesterId, result.Value.Images);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Unexpected error occured while generating image url for {RequesterId}; Error: {Error}.", requesterId, ex.Message);
            await PublishFail(requesterId, "Unexpected error occured.");
        }
    }

    private async Task PublishSuccess(Guid requesterId, ImageUrlToSize[] fileUrls)
    {
        await _publisher.Publish(new ImageUploadedIntegrationEvent(requesterId, fileUrls));
    }

    private async Task PublishFail(Guid requesterId, string details)
    {
        await _publisher.Publish(new ImageUploadFailedDomainEvent(requesterId, details));
    }

    private async Task<ValueOrNull<UploadImageResponse>> UploadFileAsync(
        Guid fileId,
        byte[] imagePreview,
        ImageSize[] sizes,
        CancellationToken cancellationToken = default)
    {
        if (imagePreview is null || imagePreview.Length <= 0)
        {
            return ValueOrNull<UploadImageResponse>.Failure("Empty information about an image was provided.");
        }

        return await _imageUploader.UploadImageAsync(
            new UploadImageRequest
            {
                FileId = fileId,
                ImageAsBytes = imagePreview,
                Sizes = sizes
            },
            cancellationToken
        );
    }
}
