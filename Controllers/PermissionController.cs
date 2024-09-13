using Microsoft.AspNetCore.Mvc;

namespace user_service_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PermissionController:ControllerBase
{
    private readonly ILogger<UserController> _logger;
    
    public PermissionController(ILogger<UserController> logger)
    {
        _logger = logger;
    }
    // 权限API 设计是级联的 而非分页 一次性get完
    /// <summary>
    /// 获取用户信息 分页 @TODO:
    /// </summary>
    /// <returns>用户对象</returns>
    
    // 为了能够实现 后续于 Role RBAC的兼容设计 需要保留一个 API 用于获取具体权限信息
    // 主要解决 具体那个API 什么角色可以访问 什么角色不能访问  @TODO: 优化之后可以细节到更细的 按钮 粒度
    
    /// CRUD
    
}