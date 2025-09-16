using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Hiscary.Shared.Domain.FileStorage;
using Hiscary.Shared.Domain.Options;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain;

namespace Hiscary.Media.FileStorage;

public sealed class BlobStorageService: IBlobStorageService
{
    private readonly ServiceUrls _serviceUrls;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<BlobStorageService> _logger;

    public BlobStorageService(BlobServiceClient blobServiceClient, ServiceUrls jwtSettings, ILogger<BlobStorageService> blobStorageService)
    {
        ArgumentNullException.ThrowIfNull(blobServiceClient);
        _blobServiceClient = blobServiceClient;
        _serviceUrls = jwtSettings;
        _logger = blobStorageService;
    }

    public async Task<ValueOrNull<string>> UploadAsync(
        string containerName,
        string blobName,
        byte[] data,
        string contentType = "application/octet-stream",
        CancellationToken cancellationToken = default)
    {
        if (data is null || data.Length == 0)
        {
            return ValueOrNull<string>.Failure("Empty data provided.");
        }

        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

            var blobClient = containerClient.GetBlobClient(blobName);

            await using var stream = new MemoryStream(data);
            await blobClient.UploadAsync(
                stream,
                new BlobHttpHeaders { ContentType = contentType },
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Unexpected error occured while generating image url for {BlobName}; Error: {Error}.", blobName, ex.Message);
            return ValueOrNull<string>.Failure("Unexpected error occured.");
        }

        return ValueOrNull<string>.Success(_serviceUrls.GetImagesUrl(blobName));
    }

    public async Task<ValueOrNull<FileNameToUrlDictionary>> UploadMultipleAsync(
        string containerName,
        List<FileWithData> files,
        string contentType = "application/octet-stream",
        CancellationToken cancellationToken = default)
    {
        if (files is null || files.Count == 0)
        {
            return ValueOrNull<FileNameToUrlDictionary>.Failure("No files provided.");
        }

        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

            var uploadTasks = files.Select(async file =>
            {
                try
                {
                    var blobClient = containerClient.GetBlobClient(file.Name);
                    await using var stream = new MemoryStream(file.Data);
                    await blobClient.UploadAsync(
                        stream,
                        new BlobHttpHeaders { ContentType = contentType },
                        cancellationToken: cancellationToken);

                    return (file.Name, Url: _serviceUrls.GetImagesUrl(file.Name));
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error uploading file {FileName} to container {ContainerName}.",
                        file.Name,
                        containerName);

                    return (file.Name, Url: (string?)null);
                }
            });

            var results = await Task.WhenAll(uploadTasks);

            var successful = results
                .Where(x => x.Url is not null)
                .ToDictionary(x => x.Name, x => x.Url!);

            if (successful.Count == 0)
            {
                return ValueOrNull<FileNameToUrlDictionary>.Failure("All uploads failed.");
            }

            return ValueOrNull<FileNameToUrlDictionary>.Success(new FileNameToUrlDictionary(successful));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error during batch upload to container {ContainerName}.",
                containerName);

            return ValueOrNull<FileNameToUrlDictionary>.Failure("Unexpected error occurred.");
        }
    }

    public async Task<(Stream content, string contentType)> DownloadAsync(
        string containerName,
        string blobName,
        CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync(cancellationToken))
            throw new FileNotFoundException($"Blob '{blobName}' not found in container '{containerName}'.");

        var download = await blobClient.DownloadAsync(cancellationToken);

        var memoryStream = new MemoryStream();
        await download.Value.Content.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        return (memoryStream, download.Value.Details.ContentType ?? "application/octet-stream");
    }

    public async Task<bool> ExistsAsync(
        string containerName,
        string blobName,
        CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        return await blobClient.ExistsAsync(cancellationToken);
    }
}
