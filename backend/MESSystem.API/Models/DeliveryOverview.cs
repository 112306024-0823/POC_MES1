using System.ComponentModel.DataAnnotations;

namespace MESSystem.API.Models;

/// <summary>
/// 進貨到廠預估表資料模型
/// </summary>
public class DeliveryOverview
{
    /// <summary>
    /// 記錄ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 提單號碼
    /// </summary>
    [Required]
    [StringLength(50)]
    public string BlNo { get; set; } = string.Empty;

    /// <summary>
    /// 客戶名稱
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Customer { get; set; } = string.Empty;

    /// <summary>
    /// 款式編號
    /// </summary>
    [StringLength(50)]
    public string? Style { get; set; }

    /// <summary>
    /// 採購單號
    /// </summary>
    [StringLength(50)]
    public string? PoNo { get; set; }

    /// <summary>
    /// 布卷數量
    /// </summary>
    public decimal? Rolls { get; set; }

    /// <summary>
    /// 預計出貨日
    /// </summary>
    public DateTime? Etd { get; set; }

    /// <summary>
    /// 預計到港日
    /// </summary>
    public DateTime? Eta { get; set; }

    /// <summary>
    /// 預計到廠日
    /// </summary>
    public DateTime? FtyEta { get; set; }

    /// <summary>
    /// 到廠狀態
    /// </summary>
    [Required]
    public ArriveStatus ArriveStatus { get; set; }

    /// <summary>
    /// 所屬工廠
    /// </summary>
    [Required]
    public Factory Factory { get; set; }
}

/// <summary>
/// 到廠狀態枚舉
/// </summary>
public enum ArriveStatus
{
    /// <summary>
    /// 如期或已到貨 (綠燈)
    /// </summary>
    OnTime = 1,

    /// <summary>
    /// 延遲或尚未到貨 (紅燈)
    /// </summary>
    Delayed = 2
} 