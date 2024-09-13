using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using user_service_api.Dto;
using user_service_api.Extensions;
using user_service_api.Middleware;
using user_service_api.Models;
using user_service_api.Service;

namespace user_service_api.Controllers;

[Route("api/v1/user_service/user")]
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
    
    // @TODO: 确定一下 鉴权方案 -1.获取token 信息中的角色信息，2. 查看注解是否运行访问 3.返回结果

    [HttpPost("all_user")]
    public async Task<ActionResult<ActionResult<Pagination<UserRes>>>> GetUsers(UserPage userPage)
    {
        var user =  await _userService.GetAllUsersAsync(userPage);
        return Ok(new ApiResponse<Pagination<UserRes>>(user, 200));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserRes>>> UpdateUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return Ok(new ApiResponse<UserRes>(null, 200, "没找到用户"));
        }

        return Ok(new ApiResponse<UserRes>(user));
    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<bool>> CreateUser(UserRegisterReq user)
    {

        try
        {
            var userInfo = new UserInfo
            {
                Username = user.UserName,
                Phone = user.Phone,
                Enable = user.Enable,
                ApproveStatus = ApproveStatus.NotApprove,
                Password = user.Password
            };

            var (ok, message) = await _userService.AddUserAsync(userInfo);
            return Ok(new ApiResponse<bool>(ok, 200,message));
        }
        catch (Exception e)
        {
            return StatusCode(500,new ApiResponse<bool>(false, 500,"服务器错误"));
        }
      
    }

    
    [HttpPut("join_community")]

    public async Task<ActionResult<ApiResponse<bool>>> JoinCommunity(UserUpdateReq newUser)
    {
        var ok = await _userService.UpdateUserAsync(newUser);
        return Ok(new ApiResponse<bool>(ok, 200, ok ? "更新成功":"更新失败"));
    }
    
    [HttpDelete("cancel/{id}")]

    public async Task<ActionResult<bool>> DeleteUser(int id)
    {
        var ok = await _userService.UpdateUserAsync(new UserUpdateReq
        {
            Id = id,
            UserInfo = new CanEditUserAttr
            {
                IsDeleted = 1
            }
        });
        return Ok(new ApiResponse<bool>(ok, 200, ok ? "更新成功":"更新失败"));
    }
    
    [HttpPut("edit")]
    public async Task<ActionResult<bool>> EditUserWithAdmin(UserUpdateReq newUser)
    {
        var ok = await _userService.UpdateUserAsync(newUser);
        return Ok(new ApiResponse<bool>(ok, 200, ok ? "更新成功":"更新失败"));
    }
}