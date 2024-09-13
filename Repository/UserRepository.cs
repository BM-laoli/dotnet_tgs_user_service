using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Dapper;
using user_service_api.Dto;
using user_service_api.Models;

namespace user_service_api.Repository;

public class UserRepository:IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IDbConnection _dbConnection;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbConnection = _context.Database.GetDbConnection();
    }

    private  IQueryable<UserInfo?>? BuilderQueryFilter(IQueryable<UserInfo?> query,UserListFilter filter)
    {
        if (!string.IsNullOrEmpty(filter.Phone))
        {
            query = query.Where(u => u != null && u.Phone == filter.Phone);
        }
        if (!string.IsNullOrEmpty(filter.Username))
        {
            query = query.Where(u => u != null && u.Username == filter.Username);
        }
        if (filter.CommunityId != null)
        {
            query = query.Where(u => u != null && u.CommunityId == filter.CommunityId);
        }
        if (!string.IsNullOrEmpty(filter.ProvinceCode))
        {
            query = query.Where(u => u != null && u.ProvinceCode == filter.ProvinceCode);
        }

        return query;
    }

    public async Task<int> GetUserCountAsync(UserListFilter? filter)
    {
        // 应用过滤条件
        var query = _context.User // 假设 UserEntity 是你的数据库实体
            .Where(u => u != null && u.IsDeleted == 0); // 假设你有一个 "IsDeleted" 属性来软删除记录

        if (filter == null)
        {
            return await query.CountAsync();
        }

        query = BuilderQueryFilter(query,filter);

        return await query.CountAsync();
    }
    
    public async Task<IEnumerable<UserRes>> GetUsersAsync(UserListFilter? filter, int page, int pageSize)
    {
        // 应用过滤条件
        var query = _context.User
            .Where(u => u.IsDeleted == 0);
      
        if (filter == null)
        {
            var  users = await  query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return users.Select(u => new UserRes // 转换为你的 DTO
            {
                Id = u.Id,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                CreatorId = u.CreatorId,
                Username = u.Username,
                Phone = u.Phone,
                Enable = u.Enable,
                ApproveStatus = u.ApproveStatus,
                RoleId = u.RoleId,
                CommunityId = u.CommunityId,
                Avatar = u.Avatar,
                ProvinceCode = u.ProvinceCode,
                CityCode = u.CityCode,
                DistrictCode = u.DistrictCode,
                StreetCode = u.StreetCode,
                Address = u.Address,
                ApproveRejectReason = u.ApproveRejectReason,
            });
        }

        query =  BuilderQueryFilter(query,filter);

        // 应用排序
        if (!string.IsNullOrEmpty(filter.Sort))
        {
            switch (filter.Sort)
            {
                case "asc":
                    query = query.OrderBy(u => u.CreatedAt);
                    break;
                case "desc":
                    query = query.OrderByDescending(u => u.CreatedAt);
                    break;
                default:
                    // 如果提供了未知的排序选项，可以选择默认排序或抛出错误
                    query = query.OrderBy(u => u.CreatedAt); // 默认按创建时间正序排序
                    break;
            }
        }

        // 应用分页2
        var  users2 = await  query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return users2.Select(u => new UserRes // 转换为你的 DTO
        {
            Id = u.Id,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt,
            CreatorId = u.CreatorId,
            Username = u.Username,
            Phone = u.Phone,
            Enable = u.Enable,
            ApproveStatus = u.ApproveStatus,
            RoleId = u.RoleId,
            CommunityId = u.CommunityId,
            Avatar = u.Avatar,
            ProvinceCode = u.ProvinceCode,
            CityCode = u.CityCode,
            DistrictCode = u.DistrictCode,
            StreetCode = u.StreetCode,
            Address = u.Address,
            ApproveRejectReason = u.ApproveRejectReason,
        });
    }
    
    public async Task<IEnumerable<UserInfo?>> GetAll()
    {
        return await _context.User.ToListAsync();
    }

    public async Task<UserInfo?> GetById(int id)
    {
        return await _context.User.FindAsync(id);
    }

    public async Task<UserInfo?> GetByPhone(string phone)
    {   
        return await _context.User.FirstOrDefaultAsync(u => u.Phone == phone);
    }

    public async Task<bool> Add(UserInfo item)
    {
        try
        {
            var info = await _context.User.AddAsync(item);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            // @TODO: logo
            return false;
        }
    }

    public async Task<bool> Update(UserUpdateReq user)
    {
        var userId = user.Id;
        var userAttr = user.UserInfo;
        var sql = @"
            UPDATE users
            SET
                Username = CASE WHEN @Username IS NOT NULL THEN @Username ELSE Username END,
                Phone = CASE WHEN @Phone IS NOT NULL THEN @Phone ELSE Phone END,
                enable = CASE WHEN @Enable IS NOT NULL THEN @Enable ELSE enable END,
                approve_status = CASE WHEN @ApproveStatus IS NOT NULL THEN @ApproveStatus ELSE approve_status END,
                role_id = CASE WHEN @RoleId IS NOT NULL THEN @RoleId ELSE role_id END,
                community_id = CASE WHEN @CommunityId IS NOT NULL THEN @CommunityId ELSE community_id END,
                avatar = CASE WHEN @Avatar IS NOT NULL THEN @Avatar ELSE avatar END,
                province_code = CASE WHEN @ProvinceCode IS NOT NULL THEN @ProvinceCode ELSE province_code END,
                city_code = CASE WHEN @CityCode IS NOT NULL THEN @CityCode ELSE city_code END,
                district_code = CASE WHEN @DistrictCode IS NOT NULL THEN @DistrictCode ELSE district_code END,
                street_code = CASE WHEN @StreetCode IS NOT NULL THEN @StreetCode ELSE street_code END,
                address = CASE WHEN @Address IS NOT NULL THEN @Address ELSE address END,
                approve_reject_reason  = CASE WHEN @ApproveRejectReason IS NOT NULL THEN @ApproveRejectReason ELSE approve_reject_reason END,
                is_deleted = CASE WHEN @IsDeleted IS NOT NULL THEN @IsDeleted ELSE is_deleted END

            WHERE Id = @UserId;
        ";

        var queryParams = new DynamicParameters();
        queryParams.Add("UserId", userId);
        queryParams.Add("Username", userAttr.Username);
        queryParams.Add("Phone", userAttr.Phone);
        queryParams.Add("Enable", userAttr.Enable);
        queryParams.Add("ApproveStatus", userAttr.ApproveStatus);
        queryParams.Add("RoleId", userAttr.RoleId);
        queryParams.Add("CommunityId", userAttr.CommunityId);
        queryParams.Add("Avatar", userAttr.Avatar);
        queryParams.Add("ProvinceCode", userAttr.ProvinceCode);
        queryParams.Add("CityCode", userAttr.CityCode);
        queryParams.Add("DistrictCode", userAttr.DistrictCode);
        queryParams.Add("StreetCode", userAttr.StreetCode);
        queryParams.Add("Address", userAttr.Address);
        queryParams.Add("ApproveRejectReason", userAttr.ApproveRejectReason);
        queryParams.Add("IsDeleted", userAttr.IsDeleted);

        var result = await _dbConnection.ExecuteAsync(sql, queryParams);
        return result > 0;
    }

    public async Task<bool>  Remove(UserInfo item)
    {
        try
        {
            _context.User.Remove(item);
            var result = await _context.SaveChangesAsync();
            return result > 0; // 如果影响的行数大于0，则返回true表示删除成功
        }
        catch (DbUpdateException ex)
        {
            // @TODO: logo
            return false; // 发生异常时返回false表示删除失败
        }
        catch (Exception ex)
        {
            // @TODO: logo
            return false; // 发生异常时返回false表示删除失败
        }
    }
}