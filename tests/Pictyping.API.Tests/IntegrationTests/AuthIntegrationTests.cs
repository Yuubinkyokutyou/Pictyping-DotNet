using Microsoft.AspNetCore.Mvc.Testing;
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

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetCurrentUser_WithoutAuthentication_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/auth/me");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CrossDomainLogin_InvalidToken_RedirectsToErrorPage()
    {
        // Configure client to not follow redirects so we can check the redirect response
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        
        var response = await client.GetAsync("/api/auth/cross-domain-login?token=invalid_token");

        // Should redirect to error page when token is invalid
        Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("picclass.com/login?migration_failed=true", response.Headers.Location?.ToString());
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
                Name = "Test User",
                Rating = 1500,
                EncryptedPassword = "hashed_password_here"
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
    }
}