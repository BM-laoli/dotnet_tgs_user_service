using Aliyun.OSS;
using Aliyun.OSS.Common;

namespace user_service_api.Middleware;

public class AliyunOSS
{
    private static OssClient _client;
    private static readonly object _lock = new object();
    private static readonly string endpoint = "https://oss-cn-hangzhou.aliyuncs.com";
    private static readonly string accessKeyId = Environment.GetEnvironmentVariable("OSS_ACCESS_KEY_ID");
    private static readonly string accessKeySecret = Environment.GetEnvironmentVariable("OSS_ACCESS_KEY_SECRET");
    private static readonly string region = "cn-hangzhou";

    public static OssClient Instance
    {
        get
        {
            if (_client == null)
            {
                lock (_lock)
                {
                    if (_client == null)
                    {
                        var conf = new ClientConfiguration
                        {
                            SignatureVersion = SignatureVersion.V4
                        };
                        _client = new OssClient(endpoint, accessKeyId, accessKeySecret, conf);
                        _client.SetRegion(region);
                    }
                }
            }
            return _client;
        }
    }
}