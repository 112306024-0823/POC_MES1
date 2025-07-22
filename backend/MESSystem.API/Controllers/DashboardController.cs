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
            var totalUsers = await _context.Users.CountAsync();
            var adminUsers = await _context.Users.CountAsync(u => u.IsAdmin);
            var totalDeliveries = await _context.DeliveryOverviews.CountAsync();
            var onTimeDeliveries = await _context.DeliveryOverviews.CountAsync(d => d.ArriveStatus == ArriveStatus.OnTime);
            var delayedDeliveries = await _context.DeliveryOverviews.CountAsync(d => d.ArriveStatus == ArriveStatus.Delayed);

            var result = new
            {
                totalUsers,
                adminUsers,
                totalDeliveries,
                onTimeDeliveries,
                delayedDeliveries
            };
            return Ok(ApiResponse<object>.CreateSuccess(result, "取得 Dashboard 統計成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得 Dashboard 統計時發生錯誤");
            return StatusCode(500, ApiResponse<object>.CreateError("系統錯誤"));
        }
    }
} 