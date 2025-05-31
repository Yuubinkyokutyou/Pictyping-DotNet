using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Pictyping.API.Controllers;
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
            new User { Id = 1, DisplayName = "Top Player", Rating = 2000, Email = "top@example.com" },
            new User { Id = 2, DisplayName = "Second Player", Rating = 1800, Email = "second@example.com" },
            new User { Id = 3, DisplayName = null, Rating = 1600, Email = "third@example.com" }
        };

        _mockRankingService.Setup(s => s.GetTopRankingsAsync(100))
            .ReturnsAsync(users);

        var result = await _controller.GetTopRankings();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        
        var rankings = okResult.Value as IEnumerable<object>;
        Assert.NotNull(rankings);
        var rankingList = rankings.ToList();
        Assert.Equal(3, rankingList.Count);
    }

    [Fact]
    public async Task GetTopRankings_CustomCount_ReturnsOkWithSpecifiedCount()
    {
        var users = new List<User>
        {
            new User { Id = 1, DisplayName = "Top Player", Rating = 2000, Email = "top@example.com" },
            new User { Id = 2, DisplayName = "Second Player", Rating = 1800, Email = "second@example.com" }
        };

        _mockRankingService.Setup(s => s.GetTopRankingsAsync(50))
            .ReturnsAsync(users);

        var result = await _controller.GetTopRankings(50);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        
        _mockRankingService.Verify(s => s.GetTopRankingsAsync(50), Times.Once);
    }

    [Fact]
    public async Task GetTopRankings_ServiceThrowsException_ReturnsInternalServerError()
    {
        _mockRankingService.Setup(s => s.GetTopRankingsAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _controller.GetTopRankings();

        var statusResult = Assert.IsType<ObjectResult>(result);
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

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        
        var response = okResult.Value;
        var userIdProperty = response.GetType().GetProperty("userId");
        var rankProperty = response.GetType().GetProperty("rank");
        
        Assert.NotNull(userIdProperty);
        Assert.NotNull(rankProperty);
        Assert.Equal(userId, userIdProperty.GetValue(response));
        Assert.Equal(rank, rankProperty.GetValue(response));
    }

    [Fact]
    public async Task GetUserRank_ServiceThrowsException_ReturnsInternalServerError()
    {
        var userId = 123;

        _mockRankingService.Setup(s => s.GetUserRankAsync(userId))
            .ThrowsAsync(new Exception("Database error"));

        var result = await _controller.GetUserRank(userId);

        var statusResult = Assert.IsType<ObjectResult>(result);
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

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        
        var response = okResult.Value;
        var messageProperty = response.GetType().GetProperty("message");
        
        Assert.NotNull(messageProperty);
        Assert.Equal("Rating updated successfully", messageProperty.GetValue(response));
        
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

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
        Assert.Equal("Internal server error", statusResult.Value);
    }
}