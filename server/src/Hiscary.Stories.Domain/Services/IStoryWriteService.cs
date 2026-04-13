using StackNucleus.DDD.Domain.ResultModels;

public interface IStoryWriteService
{
    Task<OperationResult<EntityIdResponse>> PublishStory(
        Guid userId,
        Guid libraryId,
        string title,
        string description,
        string authorName,
        IEnumerable<Guid> genreIds,
        int ageLimit,
        byte[]? imagePreview,
        bool shouldUpdateImage,
        DateTime dateWritten);

    Task<OperationResult> UpdateStory(
        Guid currentUserId,
        string callerRole,
        Guid storyId,
        string title,
        string description,
        string authorName,
        IEnumerable<Guid> genreIds,
        int ageLimit,
        byte[]? imagePreview,
        bool shouldUpdateImage,
        string? pdfFileName,
        DateTime dateWritten,
        IEnumerable<string> contents);

    Task<OperationResult> DeleteStory(Guid storyId, Guid callerId, string callerRole);

    Task<OperationResult> DeleteAudio(Guid storyId, Guid callerId, string callerRole);

    Task<OperationResult> UpdateAudio(Guid storyId, string name, byte[] audio);

    Task<OperationResult> AddComment(Guid storyId, Guid userId, string content, int score);

    Task<OperationResult> UpdateComment(Guid commentId, Guid storyId, string content, int score, Guid callerId, string callerRole);

    Task<OperationResult> DeleteComment(Guid storyId, Guid commentId, Guid callerId, string callerRole);

    Task<OperationResult> SetStoryScoreForAUser(Guid storyId, Guid userId, int score);

    Task<OperationResult> CreateGenre(string name, string description, byte[] imagePreview);

    Task<OperationResult> UpdateGenre(Guid genreId, string name, string description, byte[] imagePreview);

    Task<OperationResult> DeleteGenre(Guid genreId);
}