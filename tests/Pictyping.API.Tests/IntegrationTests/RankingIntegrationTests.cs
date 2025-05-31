using Microsoft.Extensions.DependencyInjection;
using Pictyping.Core.Entities;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Pictyping.API.Tests.IntegrationTests;

public class RankingIntegrationTests : IClassFixture<ApiTestFixture>
{
    private readonly ApiTestFixture _factory;
    private readonly HttpClient _client;

    public RankingIntegrationTests(ApiTestFixture factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetTopRankings_ReturnsSuccessAndCorrectContentType()
    {
        await SeedTestData();

        var response = await _client.GetAsync("/api/ranking");

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", 
            response.Content.Headers.ContentType?.ToString());

        var content = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrEmpty(content));
        
        var rankings = JsonSerializer.Deserialize<JsonElement>(content);
        Assert.Equal(JsonValueKind.Array, rankings.ValueKind);
        Assert.True(rankings.GetArrayLength() >= 0);
    }

    [Fact]
    public async Task GetTopRankings_WithCount_ReturnsLimitedResults()
    {
        await SeedTestData();

        var response = await _client.GetAsync("/api/ranking?count=2");

        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var rankings = JsonSerializer.Deserialize<JsonElement>(content);
        
        Assert.Equal(JsonValueKind.Array, rankings.ValueKind);
        Assert.True(rankings.GetArrayLength() <= 2);
    }

    [Fact]
    public async Task GetUserRank_ExistingUser_ReturnsUserRank()
    {
        var userId = await SeedTestData();

        var response = await _client.GetAsync($"/api/ranking/user/{userId}");

        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        
        Assert.True(result.TryGetProperty("userId", out var userIdElement));
        Assert.True(result.TryGetProperty("rank", out var rankElement));
        Assert.Equal(userId, userIdElement.GetInt32());
        Assert.True(rankElement.GetInt32() >= 0);
    }

    [Fact]
    public async Task UpdateUserRating_ValidRequest_ReturnsSuccessMessage()
    {
        var userId = await SeedTestData();
        var updateRequest = new { NewRating = 1800 };

        var response = await _client.PutAsJsonAsync($"/api/ranking/user/{userId}", updateRequest);

        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        
        Assert.True(result.TryGetProperty("message", out var messageElement));
        Assert.Equal("Rating updated successfully", messageElement.GetString());
    }

    private async Task<int> SeedTestData()
    {
        using var context = await _factory.GetDbContextAsync();
        
        if (!context.Users.Any())
        {
            var users = new[]
            {
                new User { Email = "fourth@example.com", DisplayName = "Fourth Player", Rating = 1400 },
                new User { Email = "third@example.com", DisplayName = "Third Player", Rating = 1600 },
                new User { Email = "second@example.com", DisplayName = "Second Player", Rating = 1800 },
                new User { Email = "top@example.com", DisplayName = "Top Player", Rating = 2000 }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }

        return context.Users.OrderByDescending(u => u.Rating).First().Id;
    }
}