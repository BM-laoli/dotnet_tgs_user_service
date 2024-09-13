using Microsoft.AspNetCore.Http.HttpResults;
using user_service_api.Dto;
using user_service_api.Extensions;
using user_service_api.Models;
using user_service_api.Repository;

namespace user_service_api.Service;

public class RoleService:IRoleService
{
    private readonly IRoleRepository _roleRepository;

    public RoleService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<bool> CreateRoleAsync(RoleReqAdd roleReqAdd)
    {
        var ok =  await _roleRepository.AddRoleAsync(new Role
        {
            Name = roleReqAdd.Name,
            Enable = roleReqAdd.Enable,
            Icon = roleReqAdd.Icon,
            Description = roleReqAdd.Description,
        });

        return ok > 0;
    }

    public async Task<bool> GetRoleByIdAsync(int id)
    {
        var ok = await _roleRepository.DeleteRoleAsync(id);
        return ok;
    }

    public async Task<bool> UpdateRoleAsync(RoleReqAdd roleReqAdd)
    {
        var ok =  await _roleRepository.UpdateRoleAsync(new Role
        {
            Name = roleReqAdd.Name,
            Enable = roleReqAdd.Enable,
            Icon = roleReqAdd.Icon,
            Description = roleReqAdd.Description,
        });

        return ok;
    }

    public async Task<bool> DeleteRoleAsync(int id)
    {
        return  await _roleRepository.DeleteRoleAsync(id);
    }

    public async Task<Pagination<RoleRes>> GetRolesAsync(int page, int pageSize)
    {
        var totalUsers = await _roleRepository.GetUserCountAsync();
        var roles = await _roleRepository.GetAllRolesAsync(page, pageSize);
        var pagination = new Pagination<RoleRes>
        {
            Current = page,
            PageSize = pageSize,
            TotalCount = totalUsers,
            TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize),
            Data = roles
        };
        return pagination;
    }
}