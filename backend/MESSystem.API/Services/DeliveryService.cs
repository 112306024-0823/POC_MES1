using Microsoft.EntityFrameworkCore;
using MESSystem.API.Data;
using MESSystem.API.DTOs;
using MESSystem.API.Models;

namespace MESSystem.API.Services;

/// <summary>
/// 進貨服務實作
/// </summary>
public class DeliveryService : IDeliveryService
{
    private readonly MESContext _context;
    private readonly ILogger<DeliveryService> _logger;

    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="context">資料庫上下文</param>
    /// <param name="logger">日誌</param>
    public DeliveryService(MESContext context, ILogger<DeliveryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 取得進貨到廠預估表
    /// </summary>
    /// <param name="factory">工廠別 (可選)</param>
    /// <returns>進貨資料清單</returns>
    public async Task<IEnumerable<DeliveryOverviewDto>> GetDeliveryOverviewsAsync(Factory? factory = null)
    {
        try
        {
            var query = _context.DeliveryOverviews.AsQueryable();

            if (factory.HasValue)
            {
                query = query.Where(d => d.Factory == factory.Value);
            }

            var deliveries = await query
                .OrderByDescending(d => d.FtyEta)
                .ToListAsync();

            return deliveries.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得進貨資料時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 根據ID取得進貨資料
    /// </summary>
    /// <param name="id">記錄ID</param>
    /// <returns>進貨資料</returns>
    public async Task<DeliveryOverviewDto?> GetDeliveryOverviewByIdAsync(int id)
    {
        try
        {
            var delivery = await _context.DeliveryOverviews
                .FirstOrDefaultAsync(d => d.Id == id);

            return delivery != null ? MapToDto(delivery) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得進貨資料時發生錯誤，ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// 建立進貨資料
    /// </summary>
    /// <param name="createDto">建立請求</param>
    /// <param name="factory">所屬工廠</param>
    /// <returns>建立的進貨資料</returns>
    public async Task<DeliveryOverviewDto> CreateDeliveryOverviewAsync(CreateDeliveryOverviewDto createDto, Factory factory)
    {
        try
        {
            var delivery = new DeliveryOverview
            {
                BlNo = createDto.BlNo,
                Customer = createDto.Customer,
                Style = createDto.Style,
                PoNo = createDto.PoNo,
                Rolls = createDto.Rolls,
                Etd = createDto.Etd,
                Eta = createDto.Eta,
                FtyEta = createDto.FtyEta,
                ArriveStatus = createDto.ArriveStatus,
                Factory = factory
            };

            _context.DeliveryOverviews.Add(delivery);
            await _context.SaveChangesAsync();

            _logger.LogInformation("建立進貨資料成功，ID: {Id}", delivery.Id);
            return MapToDto(delivery);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立進貨資料時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 更新進貨資料
    /// </summary>
    /// <param name="id">記錄ID</param>
    /// <param name="updateDto">更新請求</param>
    /// <returns>更新後的進貨資料</returns>
    public async Task<DeliveryOverviewDto?> UpdateDeliveryOverviewAsync(int id, CreateDeliveryOverviewDto updateDto)
    {
        try
        {
            var delivery = await _context.DeliveryOverviews
                .FirstOrDefaultAsync(d => d.Id == id);

            if (delivery == null)
            {
                return null;
            }

            delivery.BlNo = updateDto.BlNo;
            delivery.Customer = updateDto.Customer;
            delivery.Style = updateDto.Style;
            delivery.PoNo = updateDto.PoNo;
            delivery.Rolls = updateDto.Rolls;
            delivery.Etd = updateDto.Etd;
            delivery.Eta = updateDto.Eta;
            delivery.FtyEta = updateDto.FtyEta;
            delivery.ArriveStatus = updateDto.ArriveStatus;

            await _context.SaveChangesAsync();

            _logger.LogInformation("更新進貨資料成功，ID: {Id}", id);
            return MapToDto(delivery);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新進貨資料時發生錯誤，ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// 刪除進貨資料
    /// </summary>
    /// <param name="id">記錄ID</param>
    /// <returns>是否成功刪除</returns>
    public async Task<bool> DeleteDeliveryOverviewAsync(int id)
    {
        try
        {
            var delivery = await _context.DeliveryOverviews
                .FirstOrDefaultAsync(d => d.Id == id);

            if (delivery == null)
            {
                return false;
            }

            _context.DeliveryOverviews.Remove(delivery);
            await _context.SaveChangesAsync();

            _logger.LogInformation("刪除進貨資料成功，ID: {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除進貨資料時發生錯誤，ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// 將實體轉換為DTO
    /// </summary>
    /// <param name="delivery">進貨實體</param>
    /// <returns>進貨DTO</returns>
    private static DeliveryOverviewDto MapToDto(DeliveryOverview delivery)
    {
        return new DeliveryOverviewDto
        {
            Id = delivery.Id,
            BlNo = delivery.BlNo,
            Customer = delivery.Customer,
            Style = delivery.Style,
            PoNo = delivery.PoNo,
            Rolls = delivery.Rolls,
            Etd = delivery.Etd,
            Eta = delivery.Eta,
            FtyEta = delivery.FtyEta,
            ArriveStatus = delivery.ArriveStatus,
            Factory = delivery.Factory
        };
    }
} 