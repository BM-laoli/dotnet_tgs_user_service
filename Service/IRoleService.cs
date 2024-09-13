using user_service_api.Dto;
using user_service_api.Models;

namespace user_service_api.Service;

public interface IRoleService
{
   // CRUD
   Task<bool> CreateRoleAsync(RoleReqAdd roleReqAdd);
   Task<bool> GetRoleByIdAsync(int  id);
   Task<bool> UpdateRoleAsync(RoleReqAdd roleReqAdd);
   Task<bool> DeleteRoleAsync(int id);
   Task<Pagination<RoleRes>> GetRolesAsync(int page, int pageSize);
}