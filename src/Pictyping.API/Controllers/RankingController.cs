using Microsoft.AspNetCore.Mvc;
using Pictyping.Core.Interfaces;
using Pictyping.API.Models;

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
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<RankingItemDto>), 200)]
    public async Task<ActionResult<IEnumerable<RankingItemDto>>> GetTopRankings([FromQuery] int count = 100)
    {
        try
        {
            var rankings = await _rankingService.GetTopRankingsAsync(count);
            var dto = rankings.Select(user => new RankingItemDto
            {
                Id = user.Id,
                DisplayName = user.Name ?? "Anonymous",
                Rating = user.Rating,
                Email = user.Email
            });
            return Ok(dto);
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
    [Produces("application/json")]
    [ProducesResponseType(typeof(UserRankDto), 200)]
    public async Task<ActionResult<UserRankDto>> GetUserRank(int userId)
    {
        try
        {
            var rank = await _rankingService.GetUserRankAsync(userId);
            return Ok(new UserRankDto { UserId = userId, Rank = rank });
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
    [Produces("application/json")]
    [ProducesResponseType(typeof(UpdateRatingResultDto), 200)]
    public async Task<ActionResult<UpdateRatingResultDto>> UpdateUserRating(int userId, [FromBody] UpdateRatingRequest request)
    {
        try
        {
            await _rankingService.UpdateUserRatingAsync(userId, request.NewRating);
            return Ok(new UpdateRatingResultDto { Message = "Rating updated successfully" });
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
