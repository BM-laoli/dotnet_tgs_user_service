using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using user_service_api.Models;

namespace user_service_api.Repository;

public class PermissionRepository:IPermissionRepository
{
    private readonly ApplicationDbContext _context;

    public PermissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Permission?>> GetAll()
    {
        return await _context.Permission.ToListAsync();
    }

    public async Task<Permission?> GetById(int id)
    {
        return await _context.Permission.FindAsync(id);
    }

    public async Task<bool> Add(Permission item)
    {
        try
        {
            var info = await _context.Permission.AddAsync(item);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            // @TODO: logo
            return false;
        }
    }

    public async Task<bool> Update(Permission item)
    {
        try
        {
            _context.Permission.Update(item);
            var result = await _context.SaveChangesAsync();
            return result > 0; // 如果影响的行数大于0，则返回true表示更新成功
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // 处理并发更新异常，例如可以通过日志记录异常详情 也可以根据需要实现乐观并发控制
            // @TODO: logo 
            return false; // 发生异常时返回false表示更新失败
        }
        catch (Exception ex)
        {
            // @TODO: logo
            return false; // 发生异常时返回false表示更新失败
        }
    }

    public async Task<bool>  Remove(Permission item)
    {
        try
        {
            _context.Permission.Remove(item);
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