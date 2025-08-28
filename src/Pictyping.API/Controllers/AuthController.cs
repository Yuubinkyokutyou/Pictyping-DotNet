using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
    /// URLが安全なリダイレクト先かを検証
    /// </summary>
    /// <param name="url">検証するURL</param>
    /// <returns>安全な場合はtrue</returns>
    private bool IsValidReturnUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return false;
        
        // 相対URLのみ許可（/で始まる）
        if (!url.StartsWith("/"))
            return false;
        
        // プロトコルやドメインが含まれていないことを確認
        if (url.Contains("://") || url.Contains("//"))
            return false;
        
        // 危険な文字列パターンをチェック
        if (url.Contains("..") || url.Contains("\\0"))
            return false;
        
        return true;
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

            // セキュアな一時認証コードを生成
            var authCode = await _authService.GenerateAuthorizationCodeAsync(userId);

            // returnUrlを検証し、安全でない場合はデフォルトにフォールバック
            var safeReturnUrl = IsValidReturnUrl(returnUrl) ? returnUrl : "/";

            // フロントエンドへリダイレクト（トークンではなくコードを使用）
            var redirectUrl = $"{_configuration["DomainSettings:NewDomain"]}/auth/callback" +
                            $"?code={authCode}" +
                            $"&returnUrl={Uri.EscapeDataString(safeReturnUrl)}";

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
        // returnUrlを検証し、安全でない場合はデフォルトにフォールバック
        var safeReturnUrl = IsValidReturnUrl(returnUrl) ? returnUrl : "/";
        
        var redirectUrl = Url.Action(nameof(ProcessGoogleAuth), "Auth");
        var properties = new AuthenticationProperties
        { 
            RedirectUri = redirectUrl,
            Items = { { "returnUrl", safeReturnUrl } }
        };
        
        return Challenge(properties, "Google");
    }

    /// <summary>
    /// Google認証ミドルウェアがCookie認証を設定した後の処理
    /// /api/signin-googleでGoogle認証が完了した後にリダイレクトされる
    /// </summary>
    [HttpGet("google/process")]
    public async Task<IActionResult> ProcessGoogleAuth()
    {
        // Cookie認証から認証情報を取得（Google認証ミドルウェアが設定済み）
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!authenticateResult.Succeeded)
        {
            _logger.LogError("Google authentication failed");
            var errorUrl = $"{_configuration["DomainSettings:NewDomain"]}/login?error=google_auth_failed";
            return Redirect(errorUrl);
        }

        var email = authenticateResult.Principal?.FindFirst(ClaimTypes.Email)?.Value;
        var googleId = authenticateResult.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var displayName = authenticateResult.Principal?.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(googleId))
        {
            _logger.LogError("Required Google user information not found. Email: {Email}, GoogleId: {GoogleId}", email, googleId);
            var errorUrl = $"{_configuration["DomainSettings:NewDomain"]}/login?error=missing_user_info";
            return Redirect(errorUrl);
        }

        try
        {
            // OAuthアイデンティティでユーザーを作成または取得
            var user = await _authService.FindOrCreateUserByOAuthAsync("google", googleId, email, displayName);
            
            // セキュアな一時認証コードを生成
            var authCode = await _authService.GenerateAuthorizationCodeAsync(user.Id.ToString());
            
            // returnUrlを取得して検証
            var returnUrl = authenticateResult.Properties?.Items["returnUrl"];
            var safeReturnUrl = IsValidReturnUrl(returnUrl) ? returnUrl : "/";

            // フロントエンドへリダイレクト（トークンではなくコードを使用）
            var redirectUrl = $"{_configuration["DomainSettings:NewDomain"]}/auth/callback" +
                             $"?code={authCode}" +
                             $"&returnUrl={Uri.EscapeDataString(safeReturnUrl)}";
            
            return Redirect(redirectUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Google OAuth callback processing failed for email: {Email}", email);
            var errorUrl = $"{_configuration["DomainSettings:NewDomain"]}/login?error=oauth_processing_failed";
            return Redirect(errorUrl);
        }
    }

    /// <summary>
    /// 認証コードをJWTトークンと交換
    /// </summary>
    [HttpPost("exchange-code")]
    public async Task<IActionResult> ExchangeCode([FromBody] ExchangeCodeRequest request)
    {
        if (string.IsNullOrEmpty(request.Code))
        {
            return BadRequest("Authorization code is required");
        }

        try
        {
            // コードをトークンと交換
            var token = await _authService.ExchangeCodeForTokenAsync(request.Code);
            
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Invalid or expired authorization code");
            }

            // トークンからユーザーIDを取得
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.ReadJwtToken(token);
            var userId = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                // セッション情報を保存
                await _authService.SaveSessionAsync(userId, token);
                
                // ユーザー情報を取得
                var user = await _authService.GetUserByIdAsync(int.Parse(userId));
                if (user != null)
                {
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
            }

            return Ok(new { token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Code exchange failed");
            return StatusCode(500, "An error occurred while exchanging the authorization code");
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

    private Task<string> GenerateJwtToken(string userId)
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
        return Task.FromResult(tokenHandler.WriteToken(token));
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

public class ExchangeCodeRequest
{
    public string Code { get; set; } = string.Empty;
}