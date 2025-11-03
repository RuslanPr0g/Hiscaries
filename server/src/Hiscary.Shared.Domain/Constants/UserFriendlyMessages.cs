namespace Hiscary.Shared.Domain.Constants;
public static class UserFriendlyMessages
{
    public static string UsernameEmpty = "The username field cannot be empty. Please provide a valid username.";
    public static string TitleEmpty = "The tile field cannot be empty. Please provide a valid title.";
    public static string ReviewMessageCannotBeEmpty = "Your review cannot be blank. Please write a message before submitting.";
    public static string UserIsNotFound = "We couldn't find a user with the provided details. Please check your information and try again.";
    public static string PasswordMismatch = UserIsNotFound;
    public static string UserIsBanned = "Your account has been banned. Please contact support for further assistance.";
    public static string TryAgainLater = "Something went wrong. Please try again later.";
    public static string UserWithUsernameExists = "A user with this username already exists. Please choose a different username.";
    public static string UserWithEmailExists = "A user with this email already exists. Please use a different email address.";
    public static string PleaseRelogin = "Your session has expired. Please log in again to continue.";
    public static string RefreshTokenIsNotExpired = "Your session is still active. There's no need to refresh the token at this moment.";
    public static string RefreshTokenIsExpired = "Your session has expired. Please refresh your token or log in again.";
    public static string StoryWasNotFound = "The requested story could not be found. It may have been removed or does not exist.";
    public static string GenreWasNotFound = "The selected genre could not be found. Please choose a valid genre.";

    public static string NoRights = "You have no rights to do that.";
}
