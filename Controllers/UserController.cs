using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using user_service_api.Dto;
using user_service_api.Extensions;
using user_service_api.Models;
using user_service_api.Service;

namespace user_service_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController: ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;
    private readonly IRedisService _redisService;
    private readonly IRabbitMQService _rabbitMqService;
    
    public UserController(
        ApplicationDbContext context,
        IUserService userService,
        ILogger<UserController> logger,
        IRedisService redisService,
        IRabbitMQService rabbitMqService
        )
    {
        _context = context;
        _userService = userService;
        _logger = logger;
        _redisService = redisService;
        _rabbitMqService = rabbitMqService;
    }
    
    // 获取所有用户信息
    [AllowAnonymous]
    [HttpGet("/api/users")]
    public async Task<ActionResult<IEnumerable<UserInfo>>> GetUsers()
    {
        var user =  await _userService.GetAllUsersAsync();
        await _redisService.SetAsync("names", "2222222");
        // send
        await _rabbitMqService.SendAsync("testQueue", "Hello, RabbitMQ!");
        return Ok(new ApiResponse<IEnumerable<UserInfo>>(user, 200));
    }
 

    
    [AllowAnonymous]
    [HttpPost("/api/users/add")]
    public async Task<ActionResult<bool>> CreateUser(UserInfo user)
    {
        var ok = await _userService.AddUserAsync(user);
        return Ok(new ApiResponse<bool>(ok, 200));
    }

}