using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Pictyping.API.Services;

namespace Pictyping.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly Pictyping.API.Services.IAuthenticationService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IConfiguration configuration,
        Pictyping.API.Services.IAuthenticationService authService,
        ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// ドメイン間認証のためのエンドポイント
    /// 旧システムから新システムへリダイレクトされた際に使用
    /// </summary>
    [HttpGet("cross-domain-login")]
    public async Task<IActionResult> CrossDomainLogin([FromQuery] string token, [FromQuery] string? returnUrl)
    {
        try
        {
            // 旧システムから渡されたトークンを検証
            var principal = ValidateToken(token);
            if (principal == null)
            {
                return Unauthorized("Invalid token");
            }

            // ユーザー情報を取得
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID not found in token");
            }

            // 新システム用のトークンを生成
            var newToken = await GenerateJwtToken(userId);

            // Redisにセッション情報を保存（両システムで共有）
            await _authService.SaveSessionAsync(userId, newToken);

            // フロントエンドへリダイレクト（トークンを含む）
            var redirectUrl = $"{_configuration["DomainSettings:NewDomain"]}/auth/callback" +
                            $"?token={newToken}" +
                            $"&returnUrl={Uri.EscapeDataString(returnUrl ?? "/")}";

            return Redirect(redirectUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cross domain login failed");
            return BadRequest("Authentication failed");
        }
    }

    /// <summary>
    /// 新システムから旧システムへアクセスする際のリダイレクト
    /// </summary>
    [HttpGet("redirect-to-legacy")]
    [Authorize]
    public async Task<IActionResult> RedirectToLegacy([FromQuery] string targetPath)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        // 一時的な認証トークンを生成
        var temporaryToken = await _authService.GenerateTemporaryTokenAsync(userId);

        // 旧システムへリダイレクト
        var redirectUrl = $"{_configuration["DomainSettings:OldDomain"]}/auth/verify" +
                         $"?token={temporaryToken}" +
                         $"&redirect={Uri.EscapeDataString(targetPath)}";

        return Redirect(redirectUrl);
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

        var token = await GenerateJwtToken(user.Id.ToString());
        
        return Ok(new
        {
            token,
            user = new
            {
                id = user.Id,
                email = user.Email,
                displayName = user.Name,
                rating = user.Rating
            }
        });
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
        // Google認証の処理
        var authenticateResult = await HttpContext.AuthenticateAsync("Google");
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
            var token = await GenerateJwtToken(user.Id.ToString());
            
            // セッション情報を保存
            await _authService.SaveSessionAsync(user.Id.ToString(), token);

            // returnUrlがあれば取得
            var returnUrl = HttpContext.Request.Query["state"].FirstOrDefault() ?? "/";

            // フロントエンドへリダイレクト
            var redirectUrl = $"{_configuration["DomainSettings:NewDomain"]}/auth/callback" +
                             $"?token={token}" +
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

    private async Task<string> GenerateJwtToken(string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException());
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60")),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private ClaimsPrincipal? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException());

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false, // 旧システムからのトークンも受け入れる
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}