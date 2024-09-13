using user_service_api.Dto;
using user_service_api.Models;

namespace user_service_api.Service;

public interface IUserService
{
    Task<Pagination<UserRes>> GetAllUsersAsync(UserPage userPage);
    Task<UserRes?> GetUserByIdAsync(int id);
    Task<( bool success,string message)> AddUserAsync(UserInfo user);
    Task<bool> UpdateUserAsync(UserUpdateReq userUpdate);
    Task<UserInfo?> GetUserByPhone(string phone);
    bool VerifyPassword(UserInfo user, string password);
    
    // 第三方短信服务 和OSS服务 (只能放到Nodejs去处理了)
}