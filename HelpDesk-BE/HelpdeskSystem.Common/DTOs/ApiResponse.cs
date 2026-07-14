namespace HelpdeskSystem.Common.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
    {
        return new ApiResponse<T> { Success = true, Message = message, Data = data };
    }

    public static ApiResponse<T> FailureResponse(string message = "Falied")
    {
        return new ApiResponse<T> 
        { 
            Success = false, 
            Message = message, 
        };
    }

    public static ApiResponse<T> FailureResponse(string message = "Failed", List<string>? errors = null)
    {
        return new ApiResponse<T> { Success = false, Message = message, Errors = errors! };
    }

    public static ApiResponse<T> EmptyResponse(string message = "No Data Available"){
        return new ApiResponse<T> { Success = true, Message = message, Data = default };
    }
}
