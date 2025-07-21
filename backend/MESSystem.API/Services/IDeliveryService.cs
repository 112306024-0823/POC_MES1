using MESSystem.API.DTOs;
using MESSystem.API.Models;

namespace MESSystem.API.Services;

/// <summary>
/// 進貨服務介面
/// </summary>
public interface IDeliveryService
{
    /// <summary>
    /// 取得進貨到廠預估表
    /// </summary>
    /// <param name="factory">工廠別 (可選)</param>
    /// <returns>進貨資料清單</returns>
    Task<IEnumerable<DeliveryOverviewDto>> GetDeliveryOverviewsAsync(Factory? factory = null);

    /// <summary>
    /// 根據ID取得進貨資料
    /// </summary>
    /// <param name="id">記錄ID</param>
    /// <returns>進貨資料</returns>
    Task<DeliveryOverviewDto?> GetDeliveryOverviewByIdAsync(int id);

    /// <summary>
    /// 建立進貨資料
    /// </summary>
    /// <param name="createDto">建立請求</param>
    /// <param name="factory">所屬工廠</param>
    /// <returns>建立的進貨資料</returns>
    Task<DeliveryOverviewDto> CreateDeliveryOverviewAsync(CreateDeliveryOverviewDto createDto, Factory factory);

    /// <summary>
    /// 更新進貨資料
    /// </summary>
    /// <param name="id">記錄ID</param>
    /// <param name="updateDto">更新請求</param>
    /// <returns>更新後的進貨資料</returns>
    Task<DeliveryOverviewDto?> UpdateDeliveryOverviewAsync(int id, CreateDeliveryOverviewDto updateDto);

    /// <summary>
    /// 刪除進貨資料
    /// </summary>
    /// <param name="id">記錄ID</param>
    /// <returns>是否成功刪除</returns>
    Task<bool> DeleteDeliveryOverviewAsync(int id);
} 