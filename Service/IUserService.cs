using user_service_api.Models;

namespace user_service_api.Service;

public interface IUserService
{
    Task<IEnumerable<UserInfo>> GetAllUsersAsync();
    Task<UserInfo> GetUserByIdAsync(int id);
    Task<bool> AddUserAsync(UserInfo user);
    Task<bool> UpdateUserAsync(UserInfo user);
    Task<bool> RemoveUserAsync(UserInfo user);
    Task<UserInfo?> GetUserByPhone(string phone);
    bool VerifyPassword(UserInfo user, string password);
}