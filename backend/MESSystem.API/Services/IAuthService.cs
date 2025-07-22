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

    /// <summary>
    /// 使用者註冊
    /// </summary>
    /// <param name="registerRequest">註冊請求</param>
    /// <returns>註冊回應</returns>
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerRequest);

    /// <summary>
    /// 批次匯入帳號
    /// </summary>
    /// <param name="users">匯入帳號清單</param>
    /// <returns>匯入結果</returns>
    Task<ImportUsersResponseDto> ImportUsersAsync(List<ImportUserDto> users);

    /// <summary>
    /// 依帳號查詢使用者（管理者權限驗證用）
    /// </summary>
    /// <param name="username">帳號</param>
    /// <returns>使用者或 null</returns>
    Task<Models.User?> GetUserByUsernameAsync(string? username);

    Task<List<Models.User>> GetAllUsersAsync();
} 