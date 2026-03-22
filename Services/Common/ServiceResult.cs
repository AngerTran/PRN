namespace CapstoneReviewTool.Services.Common;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ServiceResult<T> Ok(T data, string message = "")
        => new() { Success = true, Data = data, Message = message };

    public static ServiceResult<T> Fail(string message)
        => new() { Success = false, Message = message };

    public static ServiceResult<T> Fail(List<string> errors)
        => new() { Success = false, Errors = errors, Message = string.Join(", ", errors) };
}

public class ServiceResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();

    public static ServiceResult Ok(string message = "")
        => new() { Success = true, Message = message };

    public static ServiceResult Fail(string message)
        => new() { Success = false, Message = message };

    public static ServiceResult Fail(List<string> errors)
        => new() { Success = false, Errors = errors, Message = string.Join(", ", errors) };
}
