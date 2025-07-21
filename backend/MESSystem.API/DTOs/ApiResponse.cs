namespace MESSystem.API.DTOs;

/// <summary>
/// 通用 API 回應格式
/// </summary>
/// <typeparam name="T">資料類型</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 回應資料
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// 訊息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 錯誤代碼
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// 建立成功回應
    /// </summary>
    /// <param name="data">資料</param>
    /// <param name="message">訊息</param>
    /// <returns>成功回應</returns>
    public static ApiResponse<T> CreateSuccess(T data, string message = "操作成功")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    /// <summary>
    /// 建立失敗回應
    /// </summary>
    /// <param name="message">錯誤訊息</param>
    /// <param name="errorCode">錯誤代碼</param>
    /// <returns>失敗回應</returns>
    public static ApiResponse<T> CreateError(string message, string? errorCode = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode
        };
    }
}

/// <summary>
/// 無資料的 API 回應格式
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 訊息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 錯誤代碼
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// 建立成功回應
    /// </summary>
    /// <param name="message">訊息</param>
    /// <returns>成功回應</returns>
    public static ApiResponse CreateSuccess(string message = "操作成功")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }

    /// <summary>
    /// 建立失敗回應
    /// </summary>
    /// <param name="message">錯誤訊息</param>
    /// <param name="errorCode">錯誤代碼</param>
    /// <returns>失敗回應</returns>
    public static ApiResponse CreateError(string message, string? errorCode = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode
        };
    }
} 