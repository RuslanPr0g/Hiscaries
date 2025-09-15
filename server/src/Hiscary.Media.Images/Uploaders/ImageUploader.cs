using Hiscary.Shared.Domain.FileStorage;
using StackNucleus.DDD.Domain.Images;
using StackNucleus.DDD.Domain.Images.Compressors;
using StackNucleus.DDD.Domain.Images.Uploaders;
using StackNucleus.DDD.Domain.ResultModels;

namespace Hiscary.Media.Images.Uploaders;

public sealed class ImageUploader(
    IImageCompressor imageCompressor,
    IBlobStorageService blobStorageService) : IImageUploader
{
    private readonly IImageCompressor _imageCompressor = imageCompressor;
    private readonly IBlobStorageService _blobStorageService = blobStorageService;

    public async Task<ValueOrNull<UploadImageResponse>> UploadImageAsync(
        UploadImageRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Extension))
        {
            return ValueOrNull<UploadImageResponse>.Failure("Empty request provided.");
        }

        if (request.ImageAsBytes is null || request.ImageAsBytes.Length <= 0)
        {
            return ValueOrNull<UploadImageResponse>.Failure("No image data provided.");
        }

        if (request.Sizes is null || request.Sizes.Length == 0)
        {
            return ValueOrNull<UploadImageResponse>.Failure("No sizes specified.");
        }

        var extension = request.Extension.TrimStart('.');
        var fileId = request.FileId;

        var files = new List<FileWithData>();

        foreach (var size in request.Sizes)
        {
            CompressionSettings settings = size switch
            {
                ImageSize.Small => new CompressionSettings { MaxWidth = 320, FormatWidth = 16, FormatHeight = 9 },
                ImageSize.Medium => new CompressionSettings { MaxWidth = 720, FormatWidth = 16, FormatHeight = 9 },
                ImageSize.Large => new CompressionSettings { MaxWidth = 1080, FormatWidth = 16, FormatHeight = 9 },
                _ => request.CompressionSettings ?? CompressionSettings.Default
            };

            var compressedImage = await _imageCompressor.CompressAsync(request.ImageAsBytes, settings);

            if (!compressedImage.HasValue || compressedImage.Value is null || !compressedImage.IsSuccess || compressedImage.Value.Length == 0)
            {
                return ValueOrNull<UploadImageResponse>.Failure(
                    $"Compression failed for size {size}: {string.Join(", ", compressedImage.Error)}");
            }

            files.Add(new FileWithData
            {
                Name = BuildFileName(fileId, extension, size),
                Data = compressedImage.Value
            });
        }

        var uploadResult = await _blobStorageService.UploadMultipleAsync(
            "images",
            files,
            contentType: $"image/{extension}",
            cancellationToken: cancellationToken);

        if (!uploadResult.HasValue || uploadResult.Value is null || !uploadResult.IsSuccess)
        {
            return ValueOrNull<UploadImageResponse>.Failure(uploadResult.Error ?? "Unexpected error occured.");
        }

        var images = request.Sizes
            .Select(size =>
                new ImageUrlToSize
                {
                    Size = size,
                    Url = uploadResult.Value.TryGetValue(BuildFileName(fileId, extension, size), out var url) ? url : string.Empty
                })
            .Where(_ => !string.IsNullOrWhiteSpace(_.Url))
            .ToArray();

        return ValueOrNull<UploadImageResponse>.Success(new UploadImageResponse
        {
            Images = images
        });
    }

    private static string BuildFileName(Guid fileId, string extension, ImageSize size)
    {
        return $"{fileId}-{size}.{extension}";
    }
}