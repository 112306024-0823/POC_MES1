using System.ComponentModel.DataAnnotations;

namespace MESSystem.API.Models;

/// <summary>
/// 使用者資料模型
/// </summary>
public class User
{
    /// <summary>
    /// 使用者ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 使用者名稱
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密碼雜湊值
    /// </summary>
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// 所屬工廠
    /// </summary>
    [Required]
    public Factory Factory { get; set; }
}

/// <summary>
/// 工廠類型枚舉
/// </summary>
public enum Factory
{
    /// <summary>
    /// TPL工廠
    /// </summary>
    TPL = 1,

    /// <summary>
    /// NVN工廠
    /// </summary>
    NVN = 2,

    /// <summary>
    /// LR工廠
    /// </summary>
    LR = 3
} 