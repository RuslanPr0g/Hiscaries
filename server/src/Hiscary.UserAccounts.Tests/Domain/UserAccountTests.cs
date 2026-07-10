using Hiscary.UserAccounts.Domain;
using Xunit;

namespace Hiscary.UserAccounts.Tests.Domain;

public sealed class UserAccountTests
{
    private static UserAccount CreateUserAccount() =>
        new(
            new UserAccountId(Guid.NewGuid()),
            username: "integration_user",
            email: "user@hiscaries.test",
            password: "hashed-password",
            birthDate: new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));

    [Fact]
    public void Ban_WhenNotAlreadyBanned_SetsIsBannedTrue()
    {
        var userAccount = CreateUserAccount();

        userAccount.Ban();

        Assert.True(userAccount.IsBanned);
    }

    [Fact]
    public void Ban_WhenAlreadyBanned_RemainsBannedAndDoesNotThrow()
    {
        var userAccount = CreateUserAccount();
        userAccount.Ban();

        var exception = Record.Exception(() => userAccount.Ban());

        Assert.Null(exception);
        Assert.True(userAccount.IsBanned);
    }

    [Fact]
    public void ValidateRefreshToken_WhenNoRefreshTokenSet_ReturnsFalse()
    {
        var userAccount = CreateUserAccount();

        var isValid = userAccount.ValidateRefreshToken("some-jwt-id");

        Assert.False(isValid);
    }
}
