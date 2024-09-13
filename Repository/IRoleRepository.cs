using user_service_api.Dto;
using user_service_api.Models;

namespace user_service_api.Repository;

public interface IRoleRepository
{
    Task<int> AddRoleAsync(Role role);
    Task<Role?> GetRoleByIdAsync(int id);
    Task<bool> UpdateRoleAsync(Role role);
    Task<bool> DeleteRoleAsync(int id);
    Task<IEnumerable<RoleRes>> GetAllRolesAsync(int page, int pageSize);
    // 计算一个总数
    Task<int> GetUserCountAsync();
}