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
} 