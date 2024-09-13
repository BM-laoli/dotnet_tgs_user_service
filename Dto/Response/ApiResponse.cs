namespace user_service_api.Dto;

public interface IApiResponse<T>
{
    public int Code { get; set; }
    public string Message { get; set; }
    public bool Success { get; set; }
    public T Data { get; set; }
}

public class ApiResponse<T> : IApiResponse<T>
{
    public int Code { get; set; }
    public string Message { get; set; }
    public bool Success { get; set; }
    public T Data { get; set; }

    public ApiResponse(T data = default, int code = 200, string message = "Success")
    {
        Data = data;
        Code = code;
        Message = message;
        Success = code >= 200 && code < 300;
    }
}