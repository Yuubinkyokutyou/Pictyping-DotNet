using Microsoft.Extensions.DependencyInjection;
using Pictyping.Core.Entities;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Pictyping.API.Tests.IntegrationTests;

public class AuthIntegrationTests : IClassFixture<ApiTestFixture>
{
    private readonly ApiTestFixture _factory;
    private readonly HttpClient _client;

    public AuthIntegrationTests(ApiTestFixture factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    // Password-based login test removed as password authentication is no longer supported

    [Fact]
    public async Task GetCurrentUser_WithoutAuthentication_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/auth/me");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CrossDomainLogin_InvalidToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/auth/cross-domain-login?token=invalid_token");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RedirectToLegacy_WithoutAuthentication_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/auth/redirect-to-legacy?targetPath=/some/path");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task SeedTestUser()
    {
        using var context = await _factory.GetDbContextAsync();
        
        if (!context.Users.Any(u => u.Email == "test@example.com"))
        {
            var user = new User
            {
                Email = "test@example.com",
                DisplayName = "Test User",
                Rating = 1500,
                // EncryptedPassword removed - no longer using password authentication
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
    }
}