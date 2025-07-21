using System.ComponentModel.DataAnnotations;
using MESSystem.API.Models;

namespace MESSystem.API.DTOs;

/// <summary>
/// 進貨到廠預估表DTO
/// </summary>
public class DeliveryOverviewDto
{
    /// <summary>
    /// 記錄ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 提單號碼
    /// </summary>
    [Required(ErrorMessage = "提單號碼為必填")]
    public string BlNo { get; set; } = string.Empty;

    /// <summary>
    /// 客戶名稱
    /// </summary>
    [Required(ErrorMessage = "客戶名稱為必填")]
    public string Customer { get; set; } = string.Empty;

    /// <summary>
    /// 款式編號
    /// </summary>
    public string? Style { get; set; }

    /// <summary>
    /// 採購單號
    /// </summary>
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
    public ArriveStatus ArriveStatus { get; set; }

    /// <summary>
    /// 所屬工廠
    /// </summary>
    public Factory Factory { get; set; }
}

/// <summary>
/// 建立進貨到廠預估表請求DTO
/// </summary>
public class CreateDeliveryOverviewDto
{
    /// <summary>
    /// 提單號碼
    /// </summary>
    [Required(ErrorMessage = "提單號碼為必填")]
    public string BlNo { get; set; } = string.Empty;

    /// <summary>
    /// 客戶名稱
    /// </summary>
    [Required(ErrorMessage = "客戶名稱為必填")]
    public string Customer { get; set; } = string.Empty;

    /// <summary>
    /// 款式編號
    /// </summary>
    public string? Style { get; set; }

    /// <summary>
    /// 採購單號
    /// </summary>
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
    public ArriveStatus ArriveStatus { get; set; }
} 