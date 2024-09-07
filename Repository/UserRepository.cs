using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using user_service_api.Models;

namespace user_service_api.Repository;

public class UserRepository:IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
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

    public async Task<bool> Update(UserInfo item)
    {
        try
        {
            _context.User.Update(item);
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