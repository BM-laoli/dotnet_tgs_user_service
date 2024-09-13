using Dapper;
using user_service_api.Dto;
using user_service_api.Extensions;
using user_service_api.Models;
using user_service_api.Repository;

namespace user_service_api.Service;

public class UserService:IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly PasswordManager _passwordManager;

    public UserService(IUserRepository userRepository, PasswordManager passwordManager)
    {
        _userRepository = userRepository;
        _passwordManager = passwordManager; 
    }

    // 获取所有用户信息
    public async Task<Pagination<UserRes>> GetAllUsersAsync(UserPage userPage)
    {
        var page = userPage.PageInfo.Current;
        var pageSize = userPage.PageInfo.PageSize;
        var totalUsers = await _userRepository.GetUserCountAsync(userPage.Filter);
        var users = await _userRepository.GetUsersAsync(userPage.Filter,page,pageSize);

        var pagination = new Pagination<UserRes>
        {
            Current = page,
            PageSize = pageSize,
            TotalCount = totalUsers,
            TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize),
            Data = users
        };
        return pagination;
    }

    // 根据ID获取用户信息
    public async Task<UserRes?> GetUserByIdAsync(int id)
    {
        var u = await _userRepository.GetById(id);
        if(u == null)
        {
            return null;
        }
        var userInfo = new UserRes
        {
            Id = u.Id,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt,
            CreatorId = u.CreatorId,
            Username = u.Username,
            Phone = u.Phone,
            Enable = u.Enable,
            ApproveStatus = u.ApproveStatus,
            RoleId = u.RoleId,
            CommunityId = u.CommunityId,
            Avatar = u.Avatar,
            ProvinceCode = u.ProvinceCode,
            CityCode = u.CityCode,
            DistrictCode = u.DistrictCode,
            StreetCode = u.StreetCode,
            Address = u.Address,
            ApproveRejectReason = u.ApproveRejectReason,
        };
        return userInfo;
    }

    // 单独查询
    public async Task<UserInfo?> GetUserByPhone(string phone)
    {
        var user = await _userRepository.GetByPhone(phone);
        return user ?? throw new InvalidOperationException($"User with ID {phone} not found.");
    }
    
    
    // 添加新用户
    public async Task<( bool success,string message)> AddUserAsync(UserInfo user)
    {
        var ex = await _userRepository.GetByPhone(user.Phone);
        if (ex != null)
        {
            return (false, "用户已存在");
        }
        user.Password = _passwordManager.HashPassword(user.Password);
        return (await _userRepository.Add(user), "");
    }

    // 更新用户信息
    public async Task<bool> UpdateUserAsync(UserUpdateReq request)
    {
        return await _userRepository.Update(request);
    }

    // 删除用户
    public async Task<bool> RemoveUserAsync(UserInfo user)
    {
        return await _userRepository.Remove(user);
    }
    
    // 解析用户password是否一致
    public bool VerifyPassword(UserInfo user, string password)
    {
        return _passwordManager.VerifyPassword(password,user.Password);
    }
}