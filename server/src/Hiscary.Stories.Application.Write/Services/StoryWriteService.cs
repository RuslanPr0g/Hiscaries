using Hiscary.Media.IntegrationEvents.Incoming;
using Hiscary.Shared.Domain.Constants;
using Hiscary.Shared.Domain.Extensions;
using Hiscary.Stories.Domain.DataAccess;
using Hiscary.Stories.Domain.Genres;
using Hiscary.Stories.Domain.Stories;
using Hiscary.Stories.IntegrationEvents.Outgoing;
using Microsoft.Extensions.Logging;
using StackNucleus.DDD.Domain.EventPublishers;
using StackNucleus.DDD.Domain.Generators;
using StackNucleus.DDD.Domain.ResultModels;

// TODO: this implementation is hideous please fix
namespace Hiscary.Stories.Application.Write.Services;

public sealed class StoryWriteService(
    IEventPublisher publisher,
    IStoryWriteRepository storyWriteRepository,
    IGenreWriteRepository genreWriteRepository,
    IIdGenerator idGenerator,
    ILogger<StoryWriteService> logger) : IStoryWriteService
{
    private readonly IEventPublisher _publisher = publisher;
    private readonly IStoryWriteRepository _repository = storyWriteRepository;
    private readonly IGenreWriteRepository _genreRepository = genreWriteRepository;
    private readonly IIdGenerator _idGenerator = idGenerator;
    private readonly ILogger<StoryWriteService> _logger = logger;

    public async Task<OperationResult> AddComment(Guid storyId, Guid userId, string content, int score)
    {
        _logger.LogInformation("Adding comment to story {StoryId} by user {UserId}", storyId, userId);
        var story = await _repository.GetById(storyId);

        if (story is null)
        {
            _logger.LogWarning("Story {StoryId} not found when adding comment", storyId);
            return OperationResult.CreateClientSideError(UserFriendlyMessages.StoryWasNotFound);
        }

        var commentId = _idGenerator.Generate((id) => new CommentId(id));
        story.AddComment(commentId, userId, content, score);
        await _repository.SaveChanges();
        _logger.LogInformation("Comment {CommentId} added to story {StoryId}", commentId, storyId);

        return OperationResult.CreateSuccess();
    }

    public async Task<OperationResult> SetStoryScoreForAUser(Guid storyId, Guid userId, int score)
    {
        _logger.LogInformation("Setting score {Score} for story {StoryId} by user {UserId}", score, storyId, userId);
        var story = await _repository.GetById(storyId);

        if (story is null)
        {
            _logger.LogWarning("Story {StoryId} not found when setting score", storyId);
            return OperationResult.CreateClientSideError(UserFriendlyMessages.StoryWasNotFound);
        }

        story.SetScoreByUser(userId, score);
        await _repository.SaveChanges();
        _logger.LogInformation("Score set for story {StoryId} with rating ID {RatingId}", storyId);

        return OperationResult.CreateSuccess();
    }

    public async Task<OperationResult> CreateGenre(string name, string description, byte[] imagePreview)
    {
        _logger.LogInformation("Creating new genre: {GenreName}", name);
        var genreId = _idGenerator.Generate((id) => new GenreId(id));
        var genre = Genre.Create(genreId, name, description, imagePreview);

        await _genreRepository.Add(genre);
        await _genreRepository.SaveChanges();
        _logger.LogInformation("Genre created with ID {GenreId}", genreId);

        return OperationResult.CreateSuccess();
    }

    public async Task<OperationResult> UpdateGenre(Guid genreId, string name, string description, byte[] imagePreview)
    {
        _logger.LogInformation("Updating genre {GenreId}", genreId);
        var genre = await _genreRepository.GetById(genreId);

        if (genre is null)
        {
            _logger.LogWarning("Genre {GenreId} not found when updating", genreId);
            return OperationResult.CreateClientSideError(UserFriendlyMessages.GenreWasNotFound);
        }

        genre.UpdateInformation(name, description, imagePreview);
        await _genreRepository.SaveChanges();
        _logger.LogInformation("Genre {GenreId} updated successfully", genreId);

        return OperationResult.CreateSuccess();
    }

    public async Task<OperationResult> UpdateComment(Guid commentId, Guid storyId, string content, int score)
    {
        _logger.LogInformation("Updating comment {CommentId} for story {StoryId}", commentId, storyId);
        var story = await _repository.GetById(storyId);

        if (story is null)
        {
            _logger.LogWarning("Story {StoryId} not found when updating comment", storyId);
            return OperationResult.CreateClientSideError(UserFriendlyMessages.StoryWasNotFound);
        }

        // TODO: add a check that only the owner is going to update his comment

        story.UpdateComment(commentId, content, score);
        await _repository.SaveChanges();
        _logger.LogInformation("Comment {CommentId} updated for story {StoryId}", commentId, storyId);

        return OperationResult.CreateSuccess();
    }

    public async Task<OperationResult<EntityIdResponse>> PublishStory(
        Guid userId,
        Guid libraryId,
        string title,
        string description,
        string authorName,
        IEnumerable<Guid> genreIds,
        int ageLimit,
        byte[]? imagePreview,
        bool shouldUpdateImage,
        DateTime dateWritten)
    {
        // TODO: perform this check using command to validate the data and send it to another service or smth
        // or insert this data into the jwt token somehow

        //var platformUser = await _userRepository.GetByUserAccountId(userId);
        //if (platformUser is null)
        //{
        //    _logger.LogWarning("Publishing new story is not possible: {StoryTitle} by {AuthorName}. User is not found {UserId}",
        //        title,
        //        authorName,
        //        userId);
        //    return OperationResult<EntityIdResponse>.CreateClientSideError(UserFriendlyMessages.UserIsNotFound);
        //}
        //var library = platformUser.GetCurrentLibrary();
        //if (!platformUser.IsPublisher || library is null)
        //{
        //    _logger.LogWarning("Publishing new story is not possible: {StoryTitle} by {AuthorName}. User is not a publisher {UserId}",
        //        title,
        //        authorName,
        //        userId);
        //    return OperationResult<EntityIdResponse>.CreateClientSideError(UserFriendlyMessages.NoRights);
        //}

        if (libraryId == Guid.Empty)
        {
            _logger.LogWarning("Publishing new story is not possible as the library doesn't exist.",
                title,
                authorName,
                userId);
            return OperationResult<EntityIdResponse>.CreateClientSideError(UserFriendlyMessages.NoRights);
        }

        _logger.LogInformation("Publishing new story: {StoryTitle} by {AuthorName}", title, authorName);
        var storyId = _idGenerator.Generate((id) => new StoryId(id));

        var existingGenres = await _genreRepository.GetByIds(genreIds.ToArray());

        var story = Story.Create(
            storyId,
            libraryId,
            title,
            description,
            authorName,
            existingGenres.ToList(),
            ageLimit,
            dateWritten);

        await _publisher.Publish(
            new StoryPublishedIntegrationEvent(libraryId,
                                               storyId,
                                               title,
                                               [],
                                               description,
                                               existingGenres.Select(x => x.Name).ToArray(),
                                               DateTime.UtcNow,
                                               0));

        var imageIsEmpty = imagePreview is null || imagePreview.Length <= 0;

        if (shouldUpdateImage && imageIsEmpty)
        {
            story.ClearPreviewUrl();
        }

        if (shouldUpdateImage && !imageIsEmpty && imagePreview is not null)
        {
            await _publisher.Publish(
                new ImageUploadRequestedIntegrationEvent(imagePreview, storyId, "stories", ImageSizeExtensions.AllImageSizes()));
        }

        await _repository.Add(story);
        await _repository.SaveChanges();
        _logger.LogInformation("Story published with ID {StoryId}", storyId);

        return OperationResult<EntityIdResponse>.CreateSuccess(new EntityIdResponse { Id = story.Id });
    }

    public async Task<OperationResult> UpdateStory(
        Guid currentUserId,
        Guid storyId,
        string title,
        string description,
        string authorName,
        IEnumerable<Guid> genreIds,
        int ageLimit,
        byte[]? imagePreview,
        bool shouldUpdateImage,
        DateTime dateWritten,
        IEnumerable<string> contents)
    {
        _logger.LogInformation("Updating story {StoryId}", storyId);
        var story = await _repository.GetById(storyId);

        if (story is null)
        {
            _logger.LogWarning("Story {StoryId} not found when updating", storyId);
            return OperationResult.CreateClientSideError(UserFriendlyMessages.StoryWasNotFound);
        }

        // TODO: do the check using command to validate the data and send it to another service request to another microservice
        // or insert this data into the jwt token somehow
        //if (story.Library.PlatformUserId.Value != currentUserId)
        //{
        //    _logger.LogWarning("Story {StoryId} can only be updated by its publisher or an administrator, user {UserId} tried to update not his story.",
        //        storyId, currentUserId);
        //    return OperationResult.CreateUnauthorizedError(UserFriendlyMessages.NoRights);
        //}

        var existingGenres = await _genreRepository.GetByIds(genreIds.ToArray());

        story.UpdateInformation(
            title,
            description,
            authorName,
            existingGenres.ToList(),
            ageLimit,
            DateTime.SpecifyKind(dateWritten, DateTimeKind.Utc));

        await _publisher.Publish(
            new StoryUpdatedIntegrationEvent(story.LibraryId,
                                             storyId,
                                             title,
                                             story.ImagePreviewUrl?.ToImageUrlToSize() ?? [],
                                             description,
                                             existingGenres.Select(x => x.Name).ToArray(),
                                             DateTime.UtcNow,
                                             story.UniqueReads));

        var imageIsEmpty = imagePreview is null || imagePreview.Length <= 0;

        if (shouldUpdateImage && imageIsEmpty)
        {
            story.ClearPreviewUrl();
        }

        if (shouldUpdateImage && !imageIsEmpty && imagePreview is not null)
        {
            await _publisher.Publish(
                new ImageUploadRequestedIntegrationEvent(imagePreview, storyId, "stories", ImageSizeExtensions.AllImageSizes()));
        }

        if (contents is not null && contents.Any())
        {
            story.ModifyContents(contents);
        }

        await _repository.SaveChanges();

        _logger.LogInformation("Story {StoryId} updated successfully", storyId);
        return OperationResult.CreateSuccess();
    }

    public async Task<OperationResult> UpdateAudio(Guid storyId, string name, byte[] audio)
    {
        _logger.LogInformation("Updating audio for story {StoryId}", storyId);
        var story = await _repository.GetById(storyId);

        if (story is null)
        {
            _logger.LogWarning("Story {StoryId} not found when updating audio", storyId);
            return OperationResult.CreateClientSideError(UserFriendlyMessages.StoryWasNotFound);
        }

        Guid fileId = Guid.NewGuid();
        var audioId = _idGenerator.Generate((id) => new StoryAudioId(id));
        story.SetAudio(audioId, fileId, name);

        try
        {
            // TODO: move it to the media service
            SaveByteArrayToFileWithBinaryWriter(audio, $"audios/{fileId}.mp3");
            _logger.LogInformation("Audio file saved for story {StoryId} with file ID {FileId}", storyId, fileId);

            await _repository.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving audio file for story {StoryId}", storyId);
            return OperationResult.CreateClientSideError("Error saving audio file");
        }

        return OperationResult.CreateSuccess();
    }

    public async Task<OperationResult> DeleteAudio(Guid storyId)
    {
        _logger.LogInformation("Deleting audio for story {StoryId}", storyId);
        var story = await _repository.GetById(storyId);

        if (story is null)
        {
            _logger.LogWarning("Story {StoryId} not found when deleting audio", storyId);
            return OperationResult.CreateClientSideError(UserFriendlyMessages.StoryWasNotFound);
        }

        Guid? removedAudioId = story.ClearAllAudio();

        if (removedAudioId.HasValue)
        {
            string path = $"audios/{removedAudioId}.mp3";

            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    _logger.LogInformation("Audio file deleted for story {StoryId}", storyId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting audio file for story {StoryId}", storyId);
                }
            }
            else
            {
                _logger.LogWarning("Audio file not found for story {StoryId}", storyId);
            }
        }

        await _repository.SaveChanges();

        return OperationResult.CreateSuccess();
    }

    public async Task<OperationResult> DeleteComment(Guid storyId, Guid commentId)
    {
        _logger.LogInformation("Deleting comment {CommentId} from story {StoryId}", commentId, storyId);
        var story = await _repository.GetById(storyId);

        if (story is null)
        {
            _logger.LogWarning("Story {StoryId} not found when deleting comment", storyId);
            return OperationResult.CreateClientSideError(UserFriendlyMessages.StoryWasNotFound);
        }

        // TODO: add rights check

        story.DeleteComment(commentId);
        await _repository.SaveChanges();

        _logger.LogInformation("Comment {CommentId} deleted from story {StoryId}", commentId, storyId);

        return OperationResult.CreateSuccess();
    }

    public async Task<OperationResult> DeleteGenre(Guid genreId)
    {
        _logger.LogInformation("Deleting genre {GenreId}", genreId);
        var genre = await _genreRepository.GetById(genreId);

        if (genre is null)
        {
            _logger.LogWarning("Genre {GenreId} not found when deleting", genreId);
            return OperationResult.CreateClientSideError(UserFriendlyMessages.GenreWasNotFound);
        }

        _genreRepository.Delete(genre);
        await _repository.SaveChanges();
        _logger.LogInformation("Genre {GenreId} deleted successfully", genreId);

        return OperationResult.CreateSuccess();
    }

    public async Task<OperationResult> DeleteStory(Guid storyId)
    {
        _logger.LogInformation("Deleting story {StoryId}", storyId);
        var story = await _repository.GetById(storyId);

        // TODO: add rights check

        if (story is not null)
        {
            // TODO: let's mark it as isDeleted and then have a
            // job that would remove all the isdeleted records once per month or smth
            _repository.Delete(story);
            await _repository.SaveChanges();
            _logger.LogInformation("Story {StoryId} deleted successfully", storyId);
        }
        else
        {
            _logger.LogWarning("Story {StoryId} not found when deleting", storyId);
        }

        return OperationResult.CreateSuccess();
    }

    // TODO: move it to the media service
    private static void SaveByteArrayToFileWithBinaryWriter(byte[] data, string filePath)
    {
        using BinaryWriter writer = new(File.OpenWrite(filePath));
        writer.Write(data);
    }
}