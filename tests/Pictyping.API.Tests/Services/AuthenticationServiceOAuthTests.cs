using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Pictyping.API.Services;
using Pictyping.Core.Entities;
using Pictyping.Infrastructure.Data;
using StackExchange.Redis;
using Xunit;

namespace Pictyping.API.Tests.Services;

public class AuthenticationServiceOAuthTests : IDisposable
{
    private readonly PictypingDbContext _context;
    private readonly Mock<IConnectionMultiplexer> _mockRedis;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly AuthenticationService _authService;

    public AuthenticationServiceOAuthTests()
    {
        var options = new DbContextOptionsBuilder<PictypingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PictypingDbContext(options);
        _mockRedis = new Mock<IConnectionMultiplexer>();
        _mockConfiguration = new Mock<IConfiguration>();

        _authService = new AuthenticationService(_context, _mockRedis.Object, _mockConfiguration.Object);
    }

    [Fact]
    public async Task FindOrCreateUserByOAuthAsync_NewUser_CreatesUserAndIdentity()
    {
        // Arrange
        var email = "test@example.com";
        var provider = "google";
        var providerUid = "google_123456789";

        // Act
        var user = await _authService.FindOrCreateUserByOAuthAsync(email, provider, providerUid);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
        Assert.True(user.Id > 0);

        // Verify OAuth identity was created
        var identity = await _context.OmniAuthIdentities
            .FirstOrDefaultAsync(o => o.Provider == provider && o.Uid == providerUid);
        Assert.NotNull(identity);
        Assert.Equal(email, identity.Email);
        Assert.Equal(user.Id, identity.UserId);
    }

    [Fact]
    public async Task FindOrCreateUserByOAuthAsync_ExistingOAuthIdentity_ReturnsExistingUser()
    {
        // Arrange
        var email = "test@example.com";
        var provider = "google";
        var providerUid = "google_123456789";

        // Create existing user and identity
        var existingUser = new User
        {
            Email = email,
            Name = "Test User",
            Rating = 1200,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var existingIdentity = new OmniAuthIdentity
        {
            Provider = provider,
            Uid = providerUid,
            Email = email,
            UserId = existingUser.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.OmniAuthIdentities.Add(existingIdentity);
        await _context.SaveChangesAsync();

        // Act
        var user = await _authService.FindOrCreateUserByOAuthAsync(email, provider, providerUid);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(existingUser.Id, user.Id);
        Assert.Equal(email, user.Email);
    }

    [Fact]
    public async Task FindOrCreateUserByOAuthAsync_ExistingUserNewOAuth_LinksOAuthToExistingUser()
    {
        // Arrange
        var email = "test@example.com";
        var provider = "google";
        var providerUid = "google_123456789";

        // Create existing user (without OAuth)
        var existingUser = new User
        {
            Email = email,
            EncryptedPassword = "password_hash",
            Name = "Test User",
            Rating = 1500,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        // Act
        var user = await _authService.FindOrCreateUserByOAuthAsync(email, provider, providerUid);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(existingUser.Id, user.Id);
        Assert.Equal(1500, user.Rating); // Original rating preserved

        // Verify OAuth identity was linked
        var identity = await _context.OmniAuthIdentities
            .FirstOrDefaultAsync(o => o.Provider == provider && o.Uid == providerUid);
        Assert.NotNull(identity);
        Assert.Equal(email, identity.Email);
        Assert.Equal(existingUser.Id, identity.UserId);
    }

    [Fact]
    public async Task FindOrCreateUserByOAuthAsync_EmailChange_UpdatesEmailInBothUserAndIdentity()
    {
        // Arrange
        var oldEmail = "old@example.com";
        var newEmail = "new@example.com";
        var provider = "google";
        var providerUid = "google_123456789";

        // Create existing user and identity with old email
        var existingUser = new User
        {
            Email = oldEmail,
            Name = "Test User",
            Rating = 1200,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var existingIdentity = new OmniAuthIdentity
        {
            Provider = provider,
            Uid = providerUid,
            Email = oldEmail,
            UserId = existingUser.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.OmniAuthIdentities.Add(existingIdentity);
        await _context.SaveChangesAsync();

        // Act - call with new email
        var user = await _authService.FindOrCreateUserByOAuthAsync(newEmail, provider, providerUid);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(newEmail, user.Email);

        // Verify identity email was also updated
        var updatedIdentity = await _context.OmniAuthIdentities
            .FirstOrDefaultAsync(o => o.Provider == provider && o.Uid == providerUid);
        Assert.NotNull(updatedIdentity);
        Assert.Equal(newEmail, updatedIdentity.Email);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}