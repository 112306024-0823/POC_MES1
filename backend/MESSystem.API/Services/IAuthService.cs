using MESSystem.API.DTOs;

namespace MESSystem.API.Services;

/// <summary>
/// 認證服務介面
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 使用者登入
    /// </summary>
    /// <param name="loginRequest">登入請求</param>
    /// <returns>登入回應</returns>
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest);

    /// <summary>
    /// 產生 JWT Token
    /// </summary>
    /// <param name="username">使用者名稱</param>
    /// <param name="factory">工廠別</param>
    /// <returns>JWT Token</returns>
    string GenerateJwtToken(string username, string factory);
} 