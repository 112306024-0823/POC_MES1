using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MESSystem.API.Data;
using MESSystem.API.DTOs;
using MESSystem.API.Models;
using Microsoft.EntityFrameworkCore;

namespace MESSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly MESContext _context;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(MESContext context, ILogger<DashboardController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<object>>> GetSummary()
    {
        try
        {
            _logger.LogInformation("開始取得 Dashboard 統計資料");
            
            var totalUsers = await _context.Users.CountAsync();
            _logger.LogInformation("使用者總數: {TotalUsers}", totalUsers);
            
            var adminUsers = await _context.Users.CountAsync(u => u.IsAdmin);
            _logger.LogInformation("管理員數: {AdminUsers}", adminUsers);
            
            var totalDeliveries = await _context.DeliveryOverviews.CountAsync();
            _logger.LogInformation("進貨記錄總數: {TotalDeliveries}", totalDeliveries);
            
            var onTimeDeliveries = await _context.DeliveryOverviews.CountAsync(d => d.ArriveStatus == ArriveStatus.OnTime);
            _logger.LogInformation("如期到貨數: {OnTimeDeliveries}", onTimeDeliveries);
            
            var delayedDeliveries = await _context.DeliveryOverviews.CountAsync(d => d.ArriveStatus == ArriveStatus.Delayed);
            _logger.LogInformation("延遲到貨數: {DelayedDeliveries}", delayedDeliveries);

            var result = new
            {
                totalUsers,
                adminUsers,
                totalDeliveries,
                onTimeDeliveries,
                delayedDeliveries
            };
            
            _logger.LogInformation("Dashboard 統計資料取得成功");
            return Ok(ApiResponse<object>.CreateSuccess(result, "取得 Dashboard 統計成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得 Dashboard 統計時發生錯誤: {Message}", ex.Message);
            return StatusCode(500, ApiResponse<object>.CreateError($"系統錯誤: {ex.Message}"));
        }
    }

    [HttpGet("test")]
    public ActionResult<ApiResponse<object>> Test()
    {
        _logger.LogInformation("Dashboard 測試端點被呼叫");
        return Ok(ApiResponse<object>.CreateSuccess(new { message = "Dashboard API 正常運作" }, "測試成功"));
    }
} 