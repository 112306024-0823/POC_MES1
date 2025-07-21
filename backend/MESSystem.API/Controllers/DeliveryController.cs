using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MESSystem.API.DTOs;
using MESSystem.API.Models;
using MESSystem.API.Services;

namespace MESSystem.API.Controllers;

/// <summary>
/// 進貨到廠預估表控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DeliveryController : ControllerBase
{
    private readonly IDeliveryService _deliveryService;
    private readonly ILogger<DeliveryController> _logger;

    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="deliveryService">進貨服務</param>
    /// <param name="logger">日誌</param>
    public DeliveryController(IDeliveryService deliveryService, ILogger<DeliveryController> logger)
    {
        _deliveryService = deliveryService;
        _logger = logger;
    }

    /// <summary>
    /// 取得進貨到廠預估表
    /// </summary>
    /// <param name="factory">工廠別篩選 (可選)</param>
    /// <returns>進貨資料清單</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<DeliveryOverviewDto>>>> GetDeliveryOverviews([FromQuery] Factory? factory = null)
    {
        try
        {
            // 如果沒有指定工廠，使用當前使用者的工廠
            if (!factory.HasValue)
            {
                var userFactory = HttpContext.User.FindFirst("Factory")?.Value;
                if (Enum.TryParse<Factory>(userFactory, out var currentFactory))
                {
                    factory = currentFactory;
                }
            }

            var deliveries = await _deliveryService.GetDeliveryOverviewsAsync(factory);
            return Ok(ApiResponse<IEnumerable<DeliveryOverviewDto>>.CreateSuccess(deliveries, "取得進貨資料成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得進貨資料時發生錯誤");
            return StatusCode(500, ApiResponse<IEnumerable<DeliveryOverviewDto>>.CreateError("系統錯誤"));
        }
    }

    /// <summary>
    /// 根據ID取得進貨資料
    /// </summary>
    /// <param name="id">記錄ID</param>
    /// <returns>進貨資料</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<DeliveryOverviewDto>>> GetDeliveryOverview(int id)
    {
        try
        {
            var delivery = await _deliveryService.GetDeliveryOverviewByIdAsync(id);

            if (delivery == null)
            {
                return NotFound(ApiResponse<DeliveryOverviewDto>.CreateError("找不到指定的進貨資料"));
            }

            return Ok(ApiResponse<DeliveryOverviewDto>.CreateSuccess(delivery));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得進貨資料時發生錯誤，ID: {Id}", id);
            return StatusCode(500, ApiResponse<DeliveryOverviewDto>.CreateError("系統錯誤"));
        }
    }

    /// <summary>
    /// 建立進貨資料
    /// </summary>
    /// <param name="createDto">建立請求</param>
    /// <returns>建立的進貨資料</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<DeliveryOverviewDto>>> CreateDeliveryOverview([FromBody] CreateDeliveryOverviewDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<DeliveryOverviewDto>.CreateError("請求資料格式錯誤"));
            }

            // 取得當前使用者的工廠
            var userFactory = HttpContext.User.FindFirst("Factory")?.Value;
            if (!Enum.TryParse<Factory>(userFactory, out var factory))
            {
                return BadRequest(ApiResponse<DeliveryOverviewDto>.CreateError("無法確認使用者工廠"));
            }

            var delivery = await _deliveryService.CreateDeliveryOverviewAsync(createDto, factory);
            return CreatedAtAction(nameof(GetDeliveryOverview), new { id = delivery.Id }, 
                ApiResponse<DeliveryOverviewDto>.CreateSuccess(delivery, "建立進貨資料成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立進貨資料時發生錯誤");
            return StatusCode(500, ApiResponse<DeliveryOverviewDto>.CreateError("系統錯誤"));
        }
    }

    /// <summary>
    /// 更新進貨資料
    /// </summary>
    /// <param name="id">記錄ID</param>
    /// <param name="updateDto">更新請求</param>
    /// <returns>更新後的進貨資料</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<DeliveryOverviewDto>>> UpdateDeliveryOverview(int id, [FromBody] CreateDeliveryOverviewDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<DeliveryOverviewDto>.CreateError("請求資料格式錯誤"));
            }

            var delivery = await _deliveryService.UpdateDeliveryOverviewAsync(id, updateDto);

            if (delivery == null)
            {
                return NotFound(ApiResponse<DeliveryOverviewDto>.CreateError("找不到指定的進貨資料"));
            }

            return Ok(ApiResponse<DeliveryOverviewDto>.CreateSuccess(delivery, "更新進貨資料成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新進貨資料時發生錯誤，ID: {Id}", id);
            return StatusCode(500, ApiResponse<DeliveryOverviewDto>.CreateError("系統錯誤"));
        }
    }

    /// <summary>
    /// 刪除進貨資料
    /// </summary>
    /// <param name="id">記錄ID</param>
    /// <returns>刪除結果</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteDeliveryOverview(int id)
    {
        try
        {
            var success = await _deliveryService.DeleteDeliveryOverviewAsync(id);

            if (!success)
            {
                return NotFound(ApiResponse.CreateError("找不到指定的進貨資料"));
            }

            return Ok(ApiResponse.CreateSuccess("刪除進貨資料成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除進貨資料時發生錯誤，ID: {Id}", id);
            return StatusCode(500, ApiResponse.CreateError("系統錯誤"));
        }
    }
} 