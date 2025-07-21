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
                ExpiresAt = expiresAt,
                IsAdmin = user.IsAdmin
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登入時發生錯誤: {Username}", loginRequest.Username);
            return null;
        }
    }

    /// <summary>
    /// 使用者註冊
    /// </summary>
    /// <param name="registerRequest">註冊請求</param>
    /// <returns>註冊回應</returns>
    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
    {
        // 檢查帳號是否已存在（同一廠別下）
        var exists = await _context.Users.AnyAsync(u => u.Username == registerRequest.Username && u.Factory == registerRequest.Factory);
        if (exists)
        {
            throw new InvalidOperationException("該帳號於此廠別已存在");
        }

        // 密碼處理：如未填則自動產生
        string password = string.IsNullOrWhiteSpace(registerRequest.Password)
            ? GenerateRandomPassword(8)
            : registerRequest.Password!;
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new Models.User
        {
            Username = registerRequest.Username,
            PasswordHash = passwordHash,
            Factory = registerRequest.Factory,
            IsAdmin = false
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new RegisterResponseDto
        {
            Username = user.Username,
            Factory = user.Factory,
            GeneratedPassword = string.IsNullOrWhiteSpace(registerRequest.Password) ? password : null,
            IsAdmin = false
        };
    }

    /// <summary>
    /// 批次匯入帳號
    /// </summary>
    /// <param name="users">匯入帳號清單</param>
    /// <returns>匯入結果</returns>
    public async Task<ImportUsersResponseDto> ImportUsersAsync(List<ImportUserDto> users)
    {
        var results = new List<ImportUserResultDto>();
        foreach (var u in users)
        {
            var result = new ImportUserResultDto
            {
                Username = u.Username,
                Factory = u.Factory
            };
            try
            {
                // 檢查帳號唯一性
                var exists = await _context.Users.AnyAsync(x => x.Username == u.Username && x.Factory == u.Factory);
                if (exists)
                {
                    result.Success = false;
                    result.Error = "該帳號於此廠別已存在";
                    results.Add(result);
                    continue;
                }
                // 密碼處理
                string password = string.IsNullOrWhiteSpace(u.Password) ? GenerateRandomPassword(8) : u.Password!;
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
                var user = new Models.User
                {
                    Username = u.Username,
                    PasswordHash = passwordHash,
                    Factory = u.Factory,
                    IsAdmin = false
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                result.Success = true;
                result.GeneratedPassword = string.IsNullOrWhiteSpace(u.Password) ? password : null;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = ex.Message;
            }
            results.Add(result);
        }
        return new ImportUsersResponseDto { Results = results };
    }

    /// <summary>
    /// 依帳號查詢使用者（管理者權限驗證用）
    /// </summary>
    public async Task<Models.User?> GetUserByUsernameAsync(string? username)
    {
        if (string.IsNullOrWhiteSpace(username)) return null;
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    /// <summary>
    /// 產生隨機密碼
    /// </summary>
    private static string GenerateRandomPassword(int length)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
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