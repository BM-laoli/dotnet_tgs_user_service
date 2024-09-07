using user_service_api.Dto;

namespace user_service_api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = response.StatusCode == 200 ? 500 : response.StatusCode;

            var errorResponse = new ApiResponse<object>
            {
                Data = null,
                Code = response.StatusCode,
                Message = ex.Message,
                Success = false,
            };

            await response.WriteAsJsonAsync(errorResponse);
        }
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder ExceptionHandlingMiddleware(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}