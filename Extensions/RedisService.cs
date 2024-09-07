using StackExchange.Redis;

namespace user_service_api.Extensions;

public interface IRedisService
{
    Task<bool> SetAsync(string key, string value);
    Task<string> GetAsync(string key);
    // 可以添加更多的Redis操作方法
}
public class RedisService:IRedisService
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public RedisService(string connectionString)
    {
        _redis = ConnectionMultiplexer.Connect(connectionString);
        _db = _redis.GetDatabase();
    }

    public async Task<bool> SetAsync(string key, string value)
    {
        return await _db.StringSetAsync(key, value);
    }

    public async Task<string> GetAsync(string key)
    {
        return await _db.StringGetAsync(key);
    }

    // 其他Redis操作...

    // 确保释放Redis连接
    public void Dispose()
    {
        _redis.Close();
    }
}