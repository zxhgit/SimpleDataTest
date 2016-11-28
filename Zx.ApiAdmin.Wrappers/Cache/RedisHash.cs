using ServiceStack.Redis;

namespace Zx.ApiAdmin.Wrappers.Cache
{
    public class RedisHash
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 服务器端口号
        /// </summary>
        public string Port { get; set; }

        public PooledRedisClientManager PoolRedisClientManager { get; set; }
    }
}
