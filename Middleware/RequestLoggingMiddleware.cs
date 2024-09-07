using System.Text;

namespace user_service_api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;
        context.Response.Body = new MemoryStream();

        try
        {
            await _next(context);

            // 这里可以读取 context.Response.Body 并进行处理
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            // 读取响应体内容
            var reader = new StreamReader(context.Response.Body);
            var responseBodyText = await reader.ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            // 根据需要修改响应体和状态码
            var modifiedResponseBody = ModifyResponseBody(responseBodyText);

            // 将修改后的内容写回响应流
            var bytes = Encoding.UTF8.GetBytes(modifiedResponseBody);
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            context.Response.ContentLength = bytes.Length;

            // 将响应流的位置重置回开始位置
            context.Response.Body.Seek(0, SeekOrigin.Begin);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during the request.");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync(ex.Message);
        }
        finally
        {
            // 将原始响应流的内容复制回原始的响应体流
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await context.Response.Body.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }

    private string ModifyResponseBody(string responseBodyText)
    {
        // 根据需要修改响应体内容
        return responseBodyText; // 返回修改后的内容
    }
}

public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder RequestLoggingMiddleware(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}