using StackNucleus.DDD.Domain.ResultModels;

namespace Hiscary.Media.Application.Write.Services;

public interface IMediaWriteService
{
    Task<OperationResult> UploadUserAnnotatedPdf(Guid userId, Guid storyId, byte[] bytes, CancellationToken cancellationToken);
    Task<OperationResult> DeleteUserAnnotatedPdf(Guid userId, Guid storyId, CancellationToken cancellationToken);
}
