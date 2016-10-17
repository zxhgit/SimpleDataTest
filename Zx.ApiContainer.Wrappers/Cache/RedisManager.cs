using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using Zx.ApiContainer.Wrappers.Utils;
using RedisConfig = Zx.ApiContainer.Entity.Log.Automatic.RedisConfig;

namespace Zx.ApiContainer.Wrappers.Cache
{
    /// <summary>
    /// 
    /// </summary>
    public class RedisManager
    {
        private static KetamaNodeLocator _KetamaNodeLocator;

        /// <summary>
        /// redis连接池对象
        /// </summary>
        private static Dictionary<string, RedisHash> _CurrentRedisClientPool;

        /// <summary>
        /// 锁定
        /// </summary>
        static object _LockPad = new object();

        /// <summary>
        /// 默认读取池数量
        /// </summary>
        const int DefaultReadPoolSize = 200;

        /// <summary>
        /// 默认写入池数量
        /// </summary>
        const int DefaultWritePoolSize = 200;

        /// <summary>
        /// 默认Redis端口
        /// </summary>
        const int DefaultRedisPort = 6379;

        /// <summary>
        /// 
        /// </summary>
        static Dictionary<long, RedisClient> _RedisClientPool = new Dictionary<long, RedisClient>();

        /// <summary>
        /// 锁定ClientPool
        /// </summary>
        static object _LockPool = new object();



        /// <summary>
        /// 静态构造方法，初始化链接池管理对象
        /// </summary>
        static RedisManager()
        {
            CreateRedisPool();
        }

        /// <summary>
        /// 构建Redis链接池对象
        /// </summary>
        private static void CreateRedisPool()
        {

            if (_CurrentRedisClientPool == null)
            {
                lock (_LockPad)
                {
                    if (_CurrentRedisClientPool == null)
                    {
                        RedisClientManagerConfig config = new RedisClientManagerConfig();

                        config.MaxReadPoolSize = DefaultReadPoolSize;

                        string readPoolSizeKey = ConfigrationHelper.GetAppSetting("RedisReadPoolSize");

                        if (readPoolSizeKey != null)
                        {
                            config.MaxReadPoolSize = Convert.ToInt32(readPoolSizeKey);
                        }

                        config.MaxWritePoolSize = DefaultWritePoolSize;

                        string writePoolSizeKey = ConfigrationHelper.GetAppSetting("RedisWritePoolSize");

                        if (writePoolSizeKey != null)
                        {
                            config.MaxWritePoolSize = Convert.ToInt32(writePoolSizeKey);
                        }

                        config.AutoStart = true;

                        IConfigrationService service = (IConfigrationService)ServiceFactory.GetService<IConfigrationService>();

                        List<RedisConfig> redisLists = service.GetRedisConfigLists();

                        Dictionary<string, RedisHash> dicPool = new Dictionary<string, RedisHash>();

                        if (redisLists != null && redisLists.Count > 0)
                        {
                            int i = 1;
                            foreach (RedisConfig rConfig in redisLists)
                            {
                                RedisHash rHash = new RedisHash();

                                PooledRedisClientManager pool = new PooledRedisClientManager(new string[1] { rConfig.RedisHost + ":" + rConfig.RedisPort }, new string[1] { rConfig.RedisHost + ":" + rConfig.RedisPort }, config);

                                rHash.PoolRedisClientManager = pool;
                                rHash.Host = rConfig.RedisHost;
                                rHash.Port = rConfig.RedisPort;

                                string tmpKey = "RedisKey" + i.ToString();

                                if (!dicPool.Keys.Contains(tmpKey))
                                {
                                    dicPool.Add(tmpKey, rHash);
                                }

                                i++;
                            }
                        }
                        else
                        {
                            throw new Exception("没有配置Redis地址");
                        }

                        _CurrentRedisClientPool = dicPool;
                    }
                }
            }


            //Thread th = new Thread(new ThreadStart(Recycle));

            //th.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void Recycle()
        {
            //while (true)
            //{
            //    foreach (KeyValuePair<long, RedisClient> pair in _RedisClientPool)
            //    {
            //        RedisClient client = pair.Value;

            //        if (client != null)
            //        {
            //            bool connect = client.IsSocketConnected();
            //        }



            //        Console.WriteLine(connect);

            //        break;
            //    }

            //    Thread.Sleep(10*1000);
            //}

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        private static void InsertRedisClientPool(RedisClient client)
        {
            if (client == null)
            {
                return;
            }

            if (!_RedisClientPool.ContainsKey(client.Id))
            {
                lock (_LockPad)
                {
                    if (!_RedisClientPool.ContainsKey(client.Id))
                    {
                        _RedisClientPool.Add(client.Id, client);
                    }
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="client"></param>
        private static void RemoveRedisClientPool(RedisClient client)
        {
            if (client == null)
            {
                return;
            }

            if (client != null && _RedisClientPool.ContainsKey(client.Id))
            {
                lock (_LockPad)
                {
                    if (client != null && _RedisClientPool.ContainsKey(client.Id))
                    {
                        _RedisClientPool.Remove(client.Id);

                        client.Dispose();
                    }
                }
            }
        }



        /// <summary>
        /// 客户端缓存操作对象
        /// </summary>
        //public static PooledRedisClientManager.DisposablePooledClient<RedisClient> GetClient()
        //{
        //    PooledRedisClientManager.DisposablePooledClient<RedisClient> client = _CurrentRedisClientPool.GetDisposableClient<RedisClient>();

        //    InsertRedisClientPool(client.Client);

        //    return client;
        //}

        public static PooledRedisClientManager.DisposablePooledClient<RedisClient> GetClient(string redisKey)
        {
            if (_CurrentRedisClientPool != null)
            {
                if (_KetamaNodeLocator == null)
                {
                    List<RedisHash> redisHashList = new List<RedisHash>();

                    redisHashList = _CurrentRedisClientPool.Values.ToList();

                    KetamaNodeLocator locator = new KetamaNodeLocator(redisHashList);

                    _KetamaNodeLocator = locator;
                }

                PooledRedisClientManager node = _KetamaNodeLocator.GetWorkerNode(redisKey);

                PooledRedisClientManager.DisposablePooledClient<RedisClient> client = node.GetDisposableClient<RedisClient>();

                InsertRedisClientPool(client.Client);

                return client;
            }
            else
            {
                return null;
            }
        }

        public static IEnumerable<RedisClient> GetAllClients()
        {
            return _CurrentRedisClientPool.Values.ToList().Select(s => s.PoolRedisClientManager.GetDisposableClient<RedisClient>().Client);
        }
    }
}
