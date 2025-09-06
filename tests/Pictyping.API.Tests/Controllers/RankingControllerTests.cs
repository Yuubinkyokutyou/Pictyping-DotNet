using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Pictyping.API.Controllers;
using Pictyping.API.Models;
using Pictyping.Core.Entities;
using Pictyping.Core.Interfaces;
using Xunit;

namespace Pictyping.API.Tests.Controllers;

public class RankingControllerTests
{
    private readonly Mock<IRankingService> _mockRankingService;
    private readonly Mock<ILogger<RankingController>> _mockLogger;
    private readonly RankingController _controller;

    public RankingControllerTests()
    {
        _mockRankingService = new Mock<IRankingService>();
        _mockLogger = new Mock<ILogger<RankingController>>();
        _controller = new RankingController(_mockRankingService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetTopRankings_DefaultCount_ReturnsOkWithRankings()
    {
        var users = new List<User>
        {
            new User { Id = 1, Name = "Top Player", Rating = 2000, Email = "top@example.com" },
            new User { Id = 2, Name = "Second Player", Rating = 1800, Email = "second@example.com" },
            new User { Id = 3, Name = null, Rating = 1600, Email = "third@example.com" }
        };

        _mockRankingService.Setup(s => s.GetTopRankingsAsync(100))
            .ReturnsAsync(users);

        var result = await _controller.GetTopRankings();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var rankings = Assert.IsAssignableFrom<IEnumerable<RankingItemDto>>(okResult.Value);
        Assert.Equal(3, rankings.Count());
    }

    [Fact]
    public async Task GetTopRankings_CustomCount_ReturnsOkWithSpecifiedCount()
    {
        var users = new List<User>
        {
            new User { Id = 1, Name = "Top Player", Rating = 2000, Email = "top@example.com" },
            new User { Id = 2, Name = "Second Player", Rating = 1800, Email = "second@example.com" }
        };

        _mockRankingService.Setup(s => s.GetTopRankingsAsync(50))
            .ReturnsAsync(users);

        var result = await _controller.GetTopRankings(50);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult.Value);
        
        _mockRankingService.Verify(s => s.GetTopRankingsAsync(50), Times.Once);
    }

    [Fact]
    public async Task GetTopRankings_ServiceThrowsException_ReturnsInternalServerError()
    {
        _mockRankingService.Setup(s => s.GetTopRankingsAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _controller.GetTopRankings();

        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
        Assert.Equal("Internal server error", statusResult.Value);
    }

    [Fact]
    public async Task GetUserRank_ValidUserId_ReturnsOkWithRank()
    {
        var userId = 123;
        var rank = 42;

        _mockRankingService.Setup(s => s.GetUserRankAsync(userId))
            .ReturnsAsync(rank);

        var result = await _controller.GetUserRank(userId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<UserRankDto>(okResult.Value);
        Assert.Equal(userId, dto.UserId);
        Assert.Equal(rank, dto.Rank);
    }

    [Fact]
    public async Task GetUserRank_ServiceThrowsException_ReturnsInternalServerError()
    {
        var userId = 123;

        _mockRankingService.Setup(s => s.GetUserRankAsync(userId))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _controller.GetUserRank(userId);

        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
        Assert.Equal("Internal server error", statusResult.Value);
    }

    [Fact]
    public async Task UpdateUserRating_ValidRequest_ReturnsOkWithSuccessMessage()
    {
        var userId = 123;
        var request = new UpdateRatingRequest { NewRating = 1800 };

        _mockRankingService.Setup(s => s.UpdateUserRatingAsync(userId, request.NewRating))
            .Returns(Task.CompletedTask);

        var result = await _controller.UpdateUserRating(userId, request);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<UpdateRatingResultDto>(okResult.Value);
        Assert.Equal("Rating updated successfully", dto.Message);
        
        _mockRankingService.Verify(s => s.UpdateUserRatingAsync(userId, request.NewRating), Times.Once);
    }

    [Fact]
    public async Task UpdateUserRating_ServiceThrowsException_ReturnsInternalServerError()
    {
        var userId = 123;
        var request = new UpdateRatingRequest { NewRating = 1800 };

        _mockRankingService.Setup(s => s.UpdateUserRatingAsync(userId, request.NewRating))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _controller.UpdateUserRating(userId, request);

        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
        Assert.Equal("Internal server error", statusResult.Value);
    }
}
