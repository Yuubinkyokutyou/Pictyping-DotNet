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
        // EncryptedPassword property removed - no longer using password authentication
        Assert.False(user.Guest);
        Assert.Equal(1200, user.Rating);
        Assert.False(user.Admin);
        Assert.NotNull(user.TypingMatchesAsPlayer);
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
    public void User_SetDisplayName_UpdatesDisplayName()
    {
        var user = new User();
        var displayName = "TestUser";

        user.DisplayName = displayName;

        Assert.Equal(displayName, user.DisplayName);
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

    // ResetPasswordToken test removed as password authentication is no longer supported

    [Fact]
    public void User_SetPlayFabId_UpdatesPlayFabId()
    {
        var user = new User();
        var playFabId = "playfab_123";

        user.PlayFabId = playFabId;

        Assert.Equal(playFabId, user.PlayFabId);
    }
}