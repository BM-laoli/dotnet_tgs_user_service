using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using user_service_api.Dto;
using user_service_api.Extensions;
using user_service_api.Models;
using user_service_api.Service;

namespace user_service_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController:ControllerBase
{
    private readonly IZookeeperService _configuration;
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public AuthController(
        IZookeeperService configuration,
        IUserService userService,
        ILogger<UserController> logger
        )
    {
        _configuration = configuration;
        _userService = userService;
        _logger = logger;
    }

    [AllowAnonymous] // 忽略授权，允许匿名访问
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginModel login)
    {
        var user = await _userService.GetUserByPhone(login.Phone);
        if (user == null)
        {
            return Ok(new ApiResponse<string>("用户查询失败"));
        }
        
        var isValid =  _userService.VerifyPassword(user, login.Password);
        if (!isValid)
        {
            return Ok(new ApiResponse<string>("密码验证失败"));
        }
        
        var jwtSettings = await _configuration.GetAsync<JwtSettings>("/tgs_config/service/user_service/jwt.json");

       
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtSettings.Key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[] 
            {
                new Claim(ClaimTypes.Name, user.Phone),
                new Claim("userId", user.Id.ToString()),
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new { Token = tokenHandler.WriteToken(token) });
    }
}