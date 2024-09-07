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
    public async Task<IEnumerable<UserInfo>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAll();
        return users.Where(u => u != null).ToList(); // 过滤掉null值
    }

    // 根据ID获取用户信息
    public async Task<UserInfo> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetById(id);
        return user ?? throw new InvalidOperationException($"User with ID {id} not found.");
    }

    // 单独查询
    public async Task<UserInfo?> GetUserByPhone(string phone)
    {
        var user = await _userRepository.GetByPhone(phone);
        return user ?? throw new InvalidOperationException($"User with ID {phone} not found.");
    }
    
    // 带条件的分页查询
    // public async Task<PageResult<UserInfo>> GetUsersAsync(int pageIndex, int pageSize)
    // {
    // }

    // 添加新用户
    public async Task<bool> AddUserAsync(UserInfo user)
    {
        user.Password = _passwordManager.HashPassword(user.Password);
        return await _userRepository.Add(user);
    }

    // 更新用户信息
    public async Task<bool> UpdateUserAsync(UserInfo user)
    {
        return await _userRepository.Update(user);
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