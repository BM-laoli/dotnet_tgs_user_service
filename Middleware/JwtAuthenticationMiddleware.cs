using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using user_service_api.Dto;
using user_service_api.Extensions;

namespace user_service_api.Middleware;

public class JwtAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IZookeeperService _configuration;
    private  JwtSettings? _jwtSettings;


    public JwtAuthenticationMiddleware(
        RequestDelegate next,
        IZookeeperService configuration,
        IHttpContextAccessor httpContextAccessor
        )
    {
        _next = next;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _jwtSettings ??= await _configuration.GetAsync<JwtSettings>("/tgs_config/service/user_service/jwt.json");

        // 检查是否具有[AllowAnonymous]属性
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata.OfType<AllowAnonymousAttribute>().Any() ?? false)
        {
            // 如果有[AllowAnonymous]属性，则不进行JWT验证
            await _next(context);
            return;
        }

        // JWT验证逻辑
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key))
                };
                var handler = new JwtSecurityTokenHandler();
                var claims = handler.ValidateToken(token, validationParameters, out SecurityToken validatedToken)
                    .Claims;
                // 将验证后的Claims转换为Claim数组
                var claimsArray = claims.Select(c => new Claim(c.Type, c.Value)).ToArray();

                // 创建ClaimsIdentity和ClaimsPrincipal
                var identity = new ClaimsIdentity(claimsArray, "JWT");
                var user = new ClaimsPrincipal(identity);

                // 将用户信息添加到HttpContext中
                context.User = user;

                // 继续处理请求
                await _next(context);
            }
            catch
            {
                // 验证失败
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                var result = new ApiResponse<string>("Unauthorized",401);
                await context.Response.WriteAsJsonAsync(result);
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            var result = new ApiResponse<string>("Unauthorized");
            await context.Response.WriteAsJsonAsync(result);
            return;
        }
    }
}




public static class RequestCultureMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestCulture(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtAuthenticationMiddleware>();
    }
}
