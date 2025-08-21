using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Pictyping.API.Services;

namespace Pictyping.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly Pictyping.API.Services.IAuthenticationService _authService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IConfiguration configuration,
        Pictyping.API.Services.IAuthenticationService authService,
        IJwtTokenService jwtTokenService,
        ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _authService = authService;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }


    /// <summary>
    /// ログイン（通常のログイン処理）
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _authService.ValidateUserAsync(request.Email, request.Password);
        if (user == null)
        {
            return Unauthorized("Invalid credentials");
        }

        // JWTトークンを生成
        var token = _jwtTokenService.GenerateToken(user);
        
        return Ok(new
        {
            token = token,
            user = new
            {
                id = user.Id,
                email = user.Email,
                displayName = user.Name,
                rating = user.Rating,
                isAdmin = user.Admin
            }
        });
    }

    /// <summary>
    /// ログアウト
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        // JWTトークンはステートレスなので、サーバー側では特に処理は不要
        // クライアント側でトークンを削除することでログアウトとなる
        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Google OAuth ログイン開始
    /// </summary>
    [HttpGet("google/login")]
    public IActionResult GoogleLogin([FromQuery] string? returnUrl)
    {
        var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth");
        var properties = new AuthenticationProperties 
        { 
            RedirectUri = redirectUrl,
            Items = { { "returnUrl", returnUrl ?? "/" } }
        };
        
        return Challenge(properties, "Google");
    }

    /// <summary>
    /// Google OAuth コールバック
    /// </summary>
    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        // TempCookieから認証情報を取得
        var authenticateResult = await HttpContext.AuthenticateAsync("TempCookie");
        if (!authenticateResult.Succeeded)
        {
            _logger.LogError("Google authentication failed");
            return Redirect($"{_configuration["DomainSettings:NewDomain"]}/login?error=google_auth_failed");
        }

        var email = authenticateResult.Principal?.FindFirst(ClaimTypes.Email)?.Value;
        var googleId = authenticateResult.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var displayName = authenticateResult.Principal?.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(googleId))
        {
            _logger.LogError("Required Google user information not found. Email: {Email}, GoogleId: {GoogleId}", email, googleId);
            return Redirect($"{_configuration["DomainSettings:NewDomain"]}/login?error=missing_user_info");
        }

        try
        {
            // OAuthアイデンティティでユーザーを作成または取得
            var user = await _authService.FindOrCreateUserByOAuthAsync("google", googleId, email, displayName);
            
            // JWTトークンを生成
            var token = _jwtTokenService.GenerateToken(user);

            // TempCookieをクリア
            await HttpContext.SignOutAsync("TempCookie");

            // returnUrlがあれば取得
            var returnUrl = HttpContext.Request.Query["state"].FirstOrDefault() ?? "/";

            // フロントエンドへリダイレクト（トークンをクエリパラメータで渡す）
            var redirectUrl = $"{_configuration["DomainSettings:NewDomain"]}/auth/callback" +
                             $"?token={Uri.EscapeDataString(token)}" +
                             $"&returnUrl={Uri.EscapeDataString(returnUrl)}";
            
            return Redirect(redirectUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Google OAuth callback processing failed for email: {Email}", email);
            return Redirect($"{_configuration["DomainSettings:NewDomain"]}/login?error=oauth_processing_failed");
        }
    }

    /// <summary>
    /// 現在のユーザー情報を取得
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var user = await _authService.GetUserByIdAsync(int.Parse(userId));
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            id = user.Id,
            email = user.Email,
            displayName = user.Name,
            rating = user.Rating,
            isAdmin = user.Admin
        });
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}