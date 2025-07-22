using Microsoft.AspNetCore.Mvc;
using MESSystem.API.DTOs;
using MESSystem.API.Services;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using CsvHelper;
using System.Globalization;
using MESSystem.API.Models;
using Microsoft.AspNetCore.Authorization;

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
    /// 使用者註冊
    /// </summary>
    /// <param name="registerRequest">註冊請求</param>
    /// <returns>註冊結果</returns>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<RegisterResponseDto>>> Register([FromBody] RegisterRequestDto registerRequest)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<RegisterResponseDto>.CreateError("請求資料格式錯誤"));
            }
            var result = await _authService.RegisterAsync(registerRequest);
            return Ok(ApiResponse<RegisterResponseDto>.CreateSuccess(result, "註冊成功"));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<RegisterResponseDto>.CreateError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "註冊時發生系統錯誤");
            return StatusCode(500, ApiResponse<RegisterResponseDto>.CreateError("系統錯誤，請稍後再試"));
        }
    }

    /// <summary>
    /// 批次匯入帳號（僅限管理者）
    /// </summary>
    /// <param name="file">上傳檔案</param>
    /// <returns>匯入結果</returns>
    [HttpPost("import-users")]
    public async Task<ActionResult<ApiResponse<ImportUsersResponseDto>>> ImportUsers([FromForm] IFormFile file)
    {
        // 權限驗證：僅限管理者
        var username = HttpContext.User.Identity?.Name;
        var user = await _authService.GetUserByUsernameAsync(username);
        if (user == null || !user.IsAdmin)
        {
            return Forbid();
        }
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<ImportUsersResponseDto>.CreateError("請上傳檔案"));
        }
        var importList = new List<ImportUserDto>();
        try
        {
            if (file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = file.OpenReadStream();
                IWorkbook workbook = new XSSFWorkbook(stream);
                var sheet = workbook.GetSheetAt(0);
                for (int i = 1; i <= sheet.LastRowNum; i++) // 跳過標題列
                {
                    var row = sheet.GetRow(i);
                    if (row == null) continue;
                    var usernameCell = row.GetCell(0)?.ToString()?.Trim() ?? string.Empty;
                    var factoryCell = row.GetCell(1)?.ToString()?.Trim() ?? string.Empty;
                    var passwordCell = row.GetCell(2)?.ToString()?.Trim();
                    if (string.IsNullOrWhiteSpace(usernameCell) || string.IsNullOrWhiteSpace(factoryCell)) continue;
                    if (!Enum.TryParse<MESSystem.API.Models.Factory>(factoryCell, true, out var factory)) continue;
                    importList.Add(new ImportUserDto
                    {
                        Username = usernameCell,
                        Factory = factory,
                        Password = passwordCell
                    });
                }
            }
            else if (file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                csv.Read(); csv.ReadHeader();
                while (csv.Read())
                {
                    var usernameCell = csv.GetField("Username")?.Trim() ?? string.Empty;
                    var factoryCell = csv.GetField("Factory")?.Trim() ?? string.Empty;
                    var passwordCell = csv.TryGetField("Password", out string? pwd) ? pwd : null;
                    if (string.IsNullOrWhiteSpace(usernameCell) || string.IsNullOrWhiteSpace(factoryCell)) continue;
                    if (!Enum.TryParse<MESSystem.API.Models.Factory>(factoryCell, true, out var factory)) continue;
                    importList.Add(new ImportUserDto
                    {
                        Username = usernameCell,
                        Factory = factory,
                        Password = passwordCell
                    });
                }
            }
            else
            {
                return BadRequest(ApiResponse<ImportUsersResponseDto>.CreateError("僅支援 .xlsx 或 .csv 檔案"));
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ImportUsersResponseDto>.CreateError($"檔案解析失敗: {ex.Message}"));
        }
        var result = await _authService.ImportUsersAsync(importList);
        return Ok(ApiResponse<ImportUsersResponseDto>.CreateSuccess(result, "匯入完成"));
    }

    /// <summary>
    /// 下載帳號匯入範本（Excel 或 CSV）
    /// </summary>
    /// <param name="type">檔案類型 xlsx/csv</param>
    [HttpGet("import-template")]
    public IActionResult DownloadImportTemplate([FromQuery] string type = "xlsx")
    {
        var headers = new[] { "Username", "Factory", "Password" };
        if (type.Equals("csv", StringComparison.OrdinalIgnoreCase))
        {
            var csv = string.Join(",", headers) + "\r\n";
            csv += "testuser, TPL, \r\n";
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "UserImportTemplate.csv");
        }
        else // 預設 Excel
        {
            using var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet("Users");
            var headerRow = sheet.CreateRow(0);
            for (int i = 0; i < headers.Length; i++)
                headerRow.CreateCell(i).SetCellValue(headers[i]);
            var sampleRow = sheet.CreateRow(1);
            sampleRow.CreateCell(0).SetCellValue("testuser");
            sampleRow.CreateCell(1).SetCellValue("TPL");
            sampleRow.CreateCell(2).SetCellValue("");
            using var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UserImportTemplate.xlsx");
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

    /// <summary>
    /// 取得所有使用者（僅帳號、廠別、是否管理員）
    /// </summary>
    [HttpGet("users")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetUsers()
    {
        try
        {
            var users = await _authService.GetAllUsersAsync();
            // 只回傳必要欄位
            var result = users.Select(u => new { u.Username, u.Factory, u.IsAdmin });
            return Ok(ApiResponse<IEnumerable<object>>.CreateSuccess(result, "取得使用者清單成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得使用者清單時發生錯誤");
            return StatusCode(500, ApiResponse<IEnumerable<object>>.CreateError("系統錯誤"));
        }
    }
} 