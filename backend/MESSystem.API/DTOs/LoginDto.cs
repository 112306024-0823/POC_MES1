using System.ComponentModel.DataAnnotations;
using MESSystem.API.Models;

namespace MESSystem.API.DTOs;

/// <summary>
/// 登入請求DTO
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// 使用者名稱
    /// </summary>
    [Required(ErrorMessage = "帳號為必填")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密碼
    /// </summary>
    [Required(ErrorMessage = "密碼為必填")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 工廠別
    /// </summary>
    [Required(ErrorMessage = "廠別為必填")]
    public Factory Factory { get; set; }
}

/// <summary>
/// 登入回應DTO
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// JWT Token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 工廠別
    /// </summary>
    public Factory Factory { get; set; }

    /// <summary>
    /// Token 過期時間
    /// </summary>
    public DateTime ExpiresAt { get; set; }
    public bool IsAdmin { get; set; } // 新增
}

/// <summary>
/// 註冊請求 DTO
/// </summary>
public class RegisterRequestDto
{
    [Required(ErrorMessage = "帳號為必填")]
    [StringLength(50, ErrorMessage = "帳號長度不可超過 50 字")]
    public string Username { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "密碼長度不可超過 100 字")]
    public string? Password { get; set; } // 可為 null，後端自動產生

    [Required(ErrorMessage = "廠別為必填")]
    public Factory Factory { get; set; }
}

/// <summary>
/// 註冊回應 DTO
/// </summary>
public class RegisterResponseDto
{
    public string Username { get; set; } = string.Empty;
    public Factory Factory { get; set; }
    public string? GeneratedPassword { get; set; } // 若自動產生密碼則回傳
    public bool IsAdmin { get; set; }
}

/// <summary>
/// 匯入帳號請求 DTO
/// </summary>
public class ImportUserDto
{
    public string Username { get; set; } = string.Empty;
    public string? Password { get; set; }
    public Factory Factory { get; set; }
}

/// <summary>
/// 匯入帳號結果 DTO
/// </summary>
public class ImportUserResultDto
{
    public string Username { get; set; } = string.Empty;
    public Factory Factory { get; set; }
    public bool Success { get; set; }
    public string? GeneratedPassword { get; set; }
    public string? Error { get; set; }
}

/// <summary>
/// 匯入帳號回應 DTO
/// </summary>
public class ImportUsersResponseDto
{
    public List<ImportUserResultDto> Results { get; set; } = new();
} 