using Microsoft.AspNetCore.Mvc;
using MESSystem.API.DTOs;
using MESSystem.API.Services;

namespace MESSystem.API.Controllers;

/// <summary>
/// 認證控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="authService">認證服務</param>
    /// <param name="logger">日誌</param>
    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// 使用者登入
    /// </summary>
    /// <param name="loginRequest">登入請求</param>
    /// <returns>登入結果</returns>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto loginRequest)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<LoginResponseDto>.CreateError("請求資料格式錯誤"));
            }

            var result = await _authService.LoginAsync(loginRequest);

            if (result == null)
            {
                return Unauthorized(ApiResponse<LoginResponseDto>.CreateError("帳號、密碼或廠別錯誤"));
            }

            _logger.LogInformation("使用者登入成功: {Username}, 工廠: {Factory}", loginRequest.Username, loginRequest.Factory);
            return Ok(ApiResponse<LoginResponseDto>.CreateSuccess(result, "登入成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登入時發生系統錯誤");
            return StatusCode(500, ApiResponse<LoginResponseDto>.CreateError("系統錯誤，請稍後再試"));
        }
    }

    /// <summary>
    /// 取得目前使用者資訊
    /// </summary>
    /// <returns>使用者資訊</returns>
    [HttpGet("me")]
    public ActionResult<ApiResponse<object>> GetCurrentUser()
    {
        try
        {
            var username = HttpContext.User.Identity?.Name;
            var factory = HttpContext.User.FindFirst("Factory")?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(ApiResponse<object>.CreateError("請先登入"));
            }

            var userInfo = new
            {
                Username = username,
                Factory = factory
            };

            return Ok(ApiResponse<object>.CreateSuccess(userInfo));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得使用者資訊時發生錯誤");
            return StatusCode(500, ApiResponse<object>.CreateError("系統錯誤"));
        }
    }
} 