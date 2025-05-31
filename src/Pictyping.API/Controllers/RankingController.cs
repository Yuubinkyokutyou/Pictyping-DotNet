using Microsoft.AspNetCore.Mvc;
using Pictyping.Core.Interfaces;

namespace Pictyping.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RankingController : ControllerBase
{
    private readonly IRankingService _rankingService;
    private readonly ILogger<RankingController> _logger;

    public RankingController(IRankingService rankingService, ILogger<RankingController> logger)
    {
        _rankingService = rankingService;
        _logger = logger;
    }

    /// <summary>
    /// 上位ランキングを取得
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetTopRankings([FromQuery] int count = 100)
    {
        try
        {
            var rankings = await _rankingService.GetTopRankingsAsync(count);
            return Ok(rankings.Select(user => new
            {
                id = user.Id,
                displayName = user.DisplayName ?? "Anonymous",
                rating = user.Rating,
                email = user.Email
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get top rankings");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// ユーザーの順位を取得
    /// </summary>
    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetUserRank(int userId)
    {
        try
        {
            var rank = await _rankingService.GetUserRankAsync(userId);
            return Ok(new { userId, rank });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user rank for user {UserId}", userId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// ユーザーのレーティングを更新
    /// </summary>
    [HttpPut("user/{userId:int}")]
    public async Task<IActionResult> UpdateUserRating(int userId, [FromBody] UpdateRatingRequest request)
    {
        try
        {
            await _rankingService.UpdateUserRatingAsync(userId, request.NewRating);
            return Ok(new { message = "Rating updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update rating for user {UserId}", userId);
            return StatusCode(500, "Internal server error");
        }
    }
}

public class UpdateRatingRequest
{
    public int NewRating { get; set; }
}