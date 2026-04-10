using StackNucleus.DDD.Domain;
using System.Net.Mime;

namespace Hiscary.Shared.Domain.FileStorage;

public interface IBlobStorageService
{
    Task<ValueOrNull<string>> UploadAsync(
        string containerName,
        string blobName,
        byte[] data,
        string contentType = MediaTypeNames.Application.Octet,
        CancellationToken cancellationToken = default);

    Task<ValueOrNull<FileNameToUrlDictionary>> UploadMultipleAsync(
        string containerName,
        List<FileWithData> files,
        string contentType = MediaTypeNames.Application.Octet,
        CancellationToken cancellationToken = default);

    Task<(Stream content, string contentType)> DownloadAsync(
        string containerName,
        string blobName,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        string containerName,
        string blobName,
        CancellationToken cancellationToken = default);

    Task<ValueOrNull<bool>> DeleteAsync(
        string containerName,
        string blobName,
        CancellationToken cancellationToken = default);
}