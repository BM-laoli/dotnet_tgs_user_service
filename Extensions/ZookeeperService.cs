using System.Text;
using Newtonsoft.Json;
using Rabbit.Zookeeper;
using Rabbit.Zookeeper.Implementation;

namespace user_service_api.Extensions;
public interface IZookeeperService
{
    Task<T> GetAsync<T>(string path);
    Task SetAsync<T>(string path, T data);
}
public class ZookeeperService:IZookeeperService
{
    private readonly ZookeeperClient _client;
    private bool _isConnected;
    public ZookeeperService(ZookeeperClientOptions options)
    {
        _client = new ZookeeperClient(options);
    }

      
    public async Task<T> GetAsync<T>(string path)
    {
        // 获取字节数组数据
        var data = await _client.GetDataAsync(path);

        // 如果数据不存在，则返回默认值
        if (data == null)
        {
            return default;
        }

        // 尝试将数据转换为字符串，然后根据类型 T 进行进一步转换
        var dataString = Encoding.UTF8.GetString(data.ToArray());

        // 如果 T 是 string，直接返回字符串
        if (typeof(T) == typeof(string))
        {
            return (T)(object)dataString;
        }
        // 尝试将字符串转换为 T 类型
        else
        {
            // 使用 JsonConvert.DeserializeObject 或其他 JSON 库进行反序列化
            return JsonConvert.DeserializeObject<T>(dataString);
        }
    }

    public async  Task SetAsync<T>(string path, T data)
    {
        // 将数据序列化为 JSON 格式
        var jsonData = JsonConvert.SerializeObject(data);
        // 将 JSON 字符串转换为字节数组
        var bytes = Encoding.UTF8.GetBytes(jsonData);
        // 调用客户端的方法设置数据
        await _client.SetDataAsync(path, bytes);
    }
}