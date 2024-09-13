using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Dapper;
using user_service_api.Dto;
using user_service_api.Models;

namespace user_service_api.Repository;

public class RoleRepository:IRoleRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IDbConnection _dbConnection;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbConnection = _context.Database.GetDbConnection();
    }

    public async Task<int> GetUserCountAsync()
    {
        var sql = @"
        SELECT COUNT(*) FROM roles;
        ";
        return await _dbConnection.ExecuteScalarAsync<int>(sql);
    }

    public async Task<int> AddRoleAsync(Role role)
    {
        var sql = @"
        INSERT INTO roles (name, enable, icon, description)
        VALUES (@Name, @Enable, @Icon, @Description);
        SELECT LAST_INSERT_ID();";
        
        return await _dbConnection.ExecuteScalarAsync<int>(sql, new
        {
            role.Name,
            role.Enable,
            role.Icon,
            role.Description
        });
    }

    public async Task<Role?> GetRoleByIdAsync(int id)
    {
        var sql = @"
        SELECT id, name, enable, icon, description
        FROM roles
        WHERE id = @Id;";
        
        return await _dbConnection.QueryFirstOrDefaultAsync<Role>(sql, new { Id = id });
    }

    public async Task<bool> UpdateRoleAsync(Role role)
    {
        var sql = @"
        UPDATE roles
        SET
            name = CASE WHEN @Name IS NOT NULL THEN @Name ELSE name END,
            enable = CASE WHEN @Enable IS NOT NULL THEN @Enable ELSE enable END,
            icon = CASE WHEN @Icon IS NOT NULL THEN @Icon ELSE icon END,
            description = CASE WHEN @Description IS NOT NULL THEN @Description ELSE description END
        WHERE id = @Id;
        ";
        
        var rowsAffected = await _dbConnection.ExecuteAsync(sql, new
        {
            role.Name,
            role.Enable,
            role.Icon,
            role.Description,
            Id = role.Id
        });
        
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteRoleAsync(int id)
    {
        var sql = @"
        DELETE FROM roles
        WHERE id = @Id;";
        
        var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<RoleRes>> GetAllRolesAsync(int page, int pageSize)
    {
        var sql = @"
        SELECT id, name, enable, icon, description
        FROM roles
        ORDER BY id
        LIMIT @PageSize OFFSET @Offset;";
        
        return await _dbConnection.QueryAsync<RoleRes>(sql, new
        {
            PageSize = pageSize,
            Offset = (page - 1) * pageSize
        });
       
    }
}