using Microsoft.EntityFrameworkCore;
using Pictyping.Core.Entities;
using Pictyping.Infrastructure.Data;
using Pictyping.Infrastructure.Services;
using Xunit;

namespace Pictyping.Core.Tests.Services;

public class UserServiceTests : IDisposable
{
    private readonly PictypingDbContext _context;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        var options = new DbContextOptionsBuilder<PictypingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PictypingDbContext(options);
        _userService = new UserService(_context);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingUser_ReturnsUser()
    {
        var user = new User
        {
            Email = "test@example.com",
            Name = "Test User",
            Rating = 1500
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = await _userService.GetByIdAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.Name, result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingUser_ReturnsNull()
    {
        var result = await _userService.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_ExistingUser_ReturnsUser()
    {
        var email = "test@example.com";
        var user = new User
        {
            Email = email,
            Name = "Test User"
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = await _userService.GetByEmailAsync(email);

        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_NonExistingUser_ReturnsNull()
    {
        var result = await _userService.GetByEmailAsync("nonexistent@example.com");

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ValidUser_CreatesAndReturnsUser()
    {
        var user = new User
        {
            Email = "newuser@example.com",
            Name = "New User",
            Rating = 1200
        };

        var result = await _userService.CreateAsync(user);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.Name, result.Name);

        var savedUser = await _context.Users.FindAsync(result.Id);
        Assert.NotNull(savedUser);
        Assert.Equal(user.Email, savedUser.Email);
    }

    [Fact]
    public async Task UpdateAsync_ExistingUser_UpdatesUser()
    {
        var user = new User
        {
            Email = "original@example.com",
            Name = "Original Name",
            Rating = 1200
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        user.Name = "Updated Name";
        user.Rating = 1600;

        var result = await _userService.UpdateAsync(user);

        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal(1600, result.Rating);

        var updatedUser = await _context.Users.FindAsync(user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal("Updated Name", updatedUser.Name);
        Assert.Equal(1600, updatedUser.Rating);
    }

    [Fact]
    public async Task DeleteAsync_ExistingUser_DeletesAndReturnsTrue()
    {
        var user = new User
        {
            Email = "todelete@example.com",
            Name = "To Delete"
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        var userId = user.Id;

        var result = await _userService.DeleteAsync(userId);

        Assert.True(result);
        var deletedUser = await _context.Users.FindAsync(userId);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingUser_ReturnsFalse()
    {
        var result = await _userService.DeleteAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        var users = new[]
        {
            new User { Email = "user1@example.com", Name = "User 1" },
            new User { Email = "user2@example.com", Name = "User 2" },
            new User { Email = "user3@example.com", Name = "User 3" }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        var result = await _userService.GetAllAsync();

        Assert.NotNull(result);
        var userList = result.ToList();
        Assert.Equal(3, userList.Count);
        Assert.Contains(userList, u => u.Email == "user1@example.com");
        Assert.Contains(userList, u => u.Email == "user2@example.com");
        Assert.Contains(userList, u => u.Email == "user3@example.com");
    }

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyCollection()
    {
        var result = await _userService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}