using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Pictyping.API.Controllers;
using Pictyping.API.Services;
using Pictyping.Core.Entities;
using System.Security.Claims;
using Xunit;

namespace Pictyping.API.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IAuthenticationService> _mockAuthService;
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockAuthService = new Mock<IAuthenticationService>();
        _mockLogger = new Mock<ILogger<AuthController>>();

        SetupConfiguration();
        
        _controller = new AuthController(
            _mockConfiguration.Object,
            _mockAuthService.Object,
            _mockLogger.Object);
    }

    private void SetupConfiguration()
    {
        _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("ThisIsASecretKeyForJwtTokenGenerationThatIsAtLeast32CharactersLong");
        _mockConfiguration.Setup(c => c["Jwt:ExpiryMinutes"]).Returns("60");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("Pictyping");
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("PictypingUsers");
        _mockConfiguration.Setup(c => c["DomainSettings:NewDomain"]).Returns("https://new.pictyping.com");
        _mockConfiguration.Setup(c => c["DomainSettings:OldDomain"]).Returns("https://pictyping.com");
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            Name = "Test User",
            Rating = 1500
        };

        _mockAuthService.Setup(s => s.ValidateUserAsync(loginRequest.Email, loginRequest.Password))
            .ReturnsAsync(user);

        var result = await _controller.Login(loginRequest);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        
        var response = okResult.Value;
        var tokenProperty = response.GetType().GetProperty("token");
        var userProperty = response.GetType().GetProperty("user");
        
        Assert.NotNull(tokenProperty);
        Assert.NotNull(userProperty);
        Assert.NotNull(tokenProperty.GetValue(response));
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "wrongpassword"
        };

        _mockAuthService.Setup(s => s.ValidateUserAsync(loginRequest.Email, loginRequest.Password))
            .ReturnsAsync((User?)null);

        var result = await _controller.Login(loginRequest);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid credentials", unauthorizedResult.Value);
    }

    [Fact]
    public async Task GetCurrentUser_ValidUser_ReturnsUserData()
    {
        var userId = "1";
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            Name = "Test User",
            Rating = 1500,
            Admin = false
        };

        _mockAuthService.Setup(s => s.GetUserByIdAsync(1))
            .ReturnsAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext()
            {
                User = claimsPrincipal
            }
        };

        var result = await _controller.GetCurrentUser();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        
        var response = okResult.Value;
        var idProperty = response.GetType().GetProperty("id");
        var emailProperty = response.GetType().GetProperty("email");
        
        Assert.NotNull(idProperty);
        Assert.NotNull(emailProperty);
        Assert.Equal(1, idProperty.GetValue(response));
        Assert.Equal("test@example.com", emailProperty.GetValue(response));
    }

    [Fact]
    public async Task GetCurrentUser_UserNotFound_ReturnsNotFound()
    {
        var userId = "999";

        _mockAuthService.Setup(s => s.GetUserByIdAsync(999))
            .ReturnsAsync((User?)null);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext()
            {
                User = claimsPrincipal
            }
        };

        var result = await _controller.GetCurrentUser();

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetCurrentUser_NoUserIdInClaims_ReturnsUnauthorized()
    {
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext()
            {
                User = claimsPrincipal
            }
        };

        var result = await _controller.GetCurrentUser();

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task RedirectToLegacy_ValidUser_ReturnsRedirect()
    {
        var userId = "1";
        var targetPath = "/some/path";
        var temporaryToken = "temp_token_123";

        _mockAuthService.Setup(s => s.GenerateTemporaryTokenAsync(userId))
            .ReturnsAsync(temporaryToken);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext()
            {
                User = claimsPrincipal
            }
        };

        var result = await _controller.RedirectToLegacy(targetPath);

        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Contains("https://pictyping.com/auth/verify", redirectResult.Url);
        Assert.Contains(temporaryToken, redirectResult.Url);
        Assert.Contains(Uri.EscapeDataString(targetPath), redirectResult.Url);
    }

    [Fact]
    public async Task RedirectToLegacy_NoUserIdInClaims_ReturnsUnauthorized()
    {
        var targetPath = "/some/path";

        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext()
            {
                User = claimsPrincipal
            }
        };

        var result = await _controller.RedirectToLegacy(targetPath);

        Assert.IsType<UnauthorizedResult>(result);
    }
}