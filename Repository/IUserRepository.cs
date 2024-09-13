using user_service_api.Dto;
using user_service_api.Models;

namespace user_service_api.Repository;

public interface IUserRepository
{
    Task<int> GetUserCountAsync(UserListFilter filter);
    Task<IEnumerable<UserRes>> GetUsersAsync(UserListFilter filter, int page, int pageSize);
    Task<IEnumerable<UserInfo?>> GetAll();
    Task<UserInfo?> GetById(int id);
    Task<bool> Add(UserInfo item);
    Task<bool> Update(UserUpdateReq item);
    Task<bool> Remove(UserInfo item);
    Task<UserInfo?> GetByPhone(string phone);
}