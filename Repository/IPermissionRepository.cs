
using user_service_api.Models;

namespace user_service_api.Repository;

public interface IPermissionRepository
{
    Task<IEnumerable<Permission?>> GetAll();
    Task<Permission?> GetById(int id);
    Task<bool> Add(Permission item);
    Task<bool> Update(Permission item);
    Task<bool> Remove(Permission item);

    // 关联的查询 @TODO:
}