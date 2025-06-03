using Pictyping.Core.Entities;
using Xunit;

namespace Pictyping.Core.Tests.Entities;

public class UserTests
{
    [Fact]
    public void User_DefaultConstructor_SetsDefaultValues()
    {
        var user = new User();

        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.EncryptedPassword);
        Assert.False(user.Guest);
        Assert.Equal(1200, user.Rating);
        Assert.False(user.Admin);
        Assert.NotNull(user.TypingMatchesAsEnemy);
        Assert.NotNull(user.OmniAuthIdentities);
    }

    [Fact]
    public void User_SetEmail_UpdatesEmail()
    {
        var user = new User();
        var email = "test@example.com";

        user.Email = email;

        Assert.Equal(email, user.Email);
    }

    [Fact]
    public void User_SetName_UpdatesName()
    {
        var user = new User();
        var name = "TestUser";

        user.Name = name;

        Assert.Equal(name, user.Name);
    }

    [Fact]
    public void User_SetRating_UpdatesRating()
    {
        var user = new User();
        var rating = 1500;

        user.Rating = rating;

        Assert.Equal(rating, user.Rating);
    }

    [Fact]
    public void User_InheritsFromBaseEntity()
    {
        var user = new User();

        Assert.IsAssignableFrom<BaseEntity>(user);
        Assert.True(user.Id >= 0);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void User_SetGuest_UpdatesGuestStatus(bool guestStatus)
    {
        var user = new User();

        user.Guest = guestStatus;

        Assert.Equal(guestStatus, user.Guest);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void User_SetAdmin_UpdatesAdminStatus(bool adminStatus)
    {
        var user = new User();

        user.Admin = adminStatus;

        Assert.Equal(adminStatus, user.Admin);
    }

    [Fact]
    public void User_SetResetPasswordToken_UpdatesToken()
    {
        var user = new User();
        var token = "reset_token_123";

        user.ResetPasswordToken = token;

        Assert.Equal(token, user.ResetPasswordToken);
    }

    [Fact]
    public void User_SetPlayfabId_UpdatesPlayfabId()
    {
        var user = new User();
        var playfabId = "playfab_123";

        user.PlayfabId = playfabId;

        Assert.Equal(playfabId, user.PlayfabId);
    }
}