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

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        var loginRequest = new
        {
            Email = "nonexistent@example.com",
            Password = "wrongpassword"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        
        // デバッグ情報を出力
        if (response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Login Status Code: {response.StatusCode}");
            Console.WriteLine($"Login Response Content: {content}");
        }

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetCurrentUser_WithoutAuthentication_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/auth/me");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // TODO: CrossDomainLogin機能は後で実装予定
    // [Fact]
    // public async Task CrossDomainLogin_InvalidToken_ReturnsUnauthorized()
    // {
    //     var response = await _client.GetAsync("/api/auth/cross-domain-login?token=invalid_token");

    //     Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    // }

    // TODO: RedirectToLegacy機能は後で実装予定
    // [Fact]
    // public async Task RedirectToLegacy_WithoutAuthentication_ReturnsUnauthorized()
    // {
    //     var response = await _client.GetAsync("/api/auth/redirect-to-legacy?targetPath=/some/path");

    //     Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    // }

    private async Task SeedTestUser()
    {
        using var context = await _factory.GetDbContextAsync();
        
        if (!context.Users.Any(u => u.Email == "test@example.com"))
        {
            var user = new User
            {
                Email = "test@example.com",
                Name = "Test User",
                Rating = 1500,
                EncryptedPassword = "hashed_password_here"
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
    }
}