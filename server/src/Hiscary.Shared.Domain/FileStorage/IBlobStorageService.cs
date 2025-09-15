using StackNucleus.DDD.Domain.ResultModels;

namespace Hiscary.Shared.Domain.FileStorage;

public interface IBlobStorageService
{
    Task<ValueOrNull<string>> UploadAsync(
        string containerName,
        string blobName,
        byte[] data,
        string contentType = "application/octet-stream",
        CancellationToken cancellationToken = default);

    Task<ValueOrNull<FileNameToUrlDictionary>> UploadMultipleAsync(
        string containerName,
        List<FileWithData> files,
        string contentType = "application/octet-stream",
        CancellationToken cancellationToken = default);

    Task<(Stream content, string contentType)> DownloadAsync(
        string containerName,
        string blobName,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        string containerName,
        string blobName,
        CancellationToken cancellationToken = default);
}