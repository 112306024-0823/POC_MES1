using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MESSystem.API.Data;
using MESSystem.API.DTOs;

namespace MESSystem.API.Services;

/// <summary>
/// 認證服務實作
/// </summary>
public class AuthService : IAuthService
{
    private readonly MESContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="context">資料庫上下文</param>
    /// <param name="configuration">設定</param>
    /// <param name="logger">日誌</param>
    public AuthService(MESContext context, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// 使用者登入
    /// </summary>
    /// <param name="loginRequest">登入請求</param>
    /// <returns>登入回應</returns>
    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest)
    {
        try
        {
            // 查詢使用者
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginRequest.Username && u.Factory == loginRequest.Factory);

            if (user == null)
            {
                _logger.LogWarning("使用者不存在: {Username}, 工廠: {Factory}", loginRequest.Username, loginRequest.Factory);
                return null;
            }

            // 驗證密碼
            if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
            {
                _logger.LogWarning("密碼錯誤: {Username}", loginRequest.Username);
                return null;
            }

            // 產生 JWT Token
            var token = GenerateJwtToken(user.Username, user.Factory.ToString());
            var expiresAt = DateTime.UtcNow.AddHours(8); // Token 有效期 8 小時

            return new LoginResponseDto
            {
                Token = token,
                Username = user.Username,
                Factory = user.Factory,
                ExpiresAt = expiresAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登入時發生錯誤: {Username}", loginRequest.Username);
            return null;
        }
    }

    /// <summary>
    /// 產生 JWT Token
    /// </summary>
    /// <param name="username">使用者名稱</param>
    /// <param name="factory">工廠別</param>
    /// <returns>JWT Token</returns>
    public string GenerateJwtToken(string username, string factory)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatShouldBeLongEnoughForSecurity123456";
        var issuer = jwtSettings["Issuer"] ?? "MESSystem";
        var audience = jwtSettings["Audience"] ?? "MESSystemUsers";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim("Factory", factory),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
} 