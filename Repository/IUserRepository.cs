using user_service_api.Models;

namespace user_service_api.Repository;

public interface IUserRepository
{
    Task<IEnumerable<UserInfo?>> GetAll();
    Task<UserInfo?> GetById(int id);
    Task<bool> Add(UserInfo item);
    Task<bool> Update(UserInfo item);
    Task<bool> Remove(UserInfo item);
    Task<UserInfo?> GetByPhone(string phone);
}