using ServiceStack.Redis;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using Zx.ApiContainer.Utilities;

namespace Zx.ApiContainer.Wrappers.Cache
{
    /// <summary>
    /// Redis操作类
    /// 2013.5.6 付林修改
    /// 使用using (IRedisClient client = GetClient()) Dispose IRedisClient 连接
    /// </summary>
    public class RedisOpt
    {
        /// <summary>
        /// 列表添加锁定
        /// </summary>
        static object _LockPad = new object();

        /// <summary>
        /// 从Redis链接池获取Redis客户端
        /// </summary>
        /// <returns></returns>
        private static PooledRedisClientManager.DisposablePooledClient<RedisClient> GetClient(string key)
        {
            return RedisManager.GetClient(key);
        }


        #region 存取单个强类型对象（底层json序列化）

        /// <summary>
        /// 将对象存入redis
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objecId">对象ID</param>
        /// <param name="obj">对象</param>
        /// <param name="ts">在某个时间过期</param>
        public static bool SetSingleCacheObject<T>(string objecid, T obj, TimeSpan ts)
        {
            string redisKey = GetRedisKey<T>(objecid);

            bool result = false;

            using (PooledRedisClientManager.DisposablePooledClient<RedisClient> client = GetClient(redisKey))
            {
                if (client != null)
                {
                    result = client.Client.Set<T>(redisKey, obj, ts);
                    //client.Client.As<T>()
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// 将对象存入redis
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objecId">对象ID</param>
        /// <param name="obj">对象</param>
        /// <param name="ts">在某个时间过期</param>
        public static bool DeleteSingleCacheObject<T>(string objecid)
        {
            string redisKey = GetRedisKey<T>(objecid);

            bool result = false;

            using (PooledRedisClientManager.DisposablePooledClient<RedisClient> client = GetClient(redisKey))
            {
                if (client != null)
                {
                    result = client.Client.Remove(redisKey);
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// 获取RedisKey
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="objectId">实际主键</param>
        /// <returns></returns>
        private static string GetRedisKey<T>(string objectId)
        {

            Type t = typeof(T);

            string typeUrl = ReflectHelper.GetTypeFullUrlWithProcessorName(t);

            string key = typeUrl + "_" + objectId;

            return key;
        }

        /// <summary>
        /// 将对象存入redis
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objecId">对象ID</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static bool SetSingleCacheObject<T>(string objecid, T obj)
        {
            bool status;

            status = SetSingleCacheObject<T>(objecid, obj, RedisConfigInfo.HighEndRedisCacheTime);

            return status;
        }

        /// <summary>
        /// 获取某个缓存对象
        /// </summary>
        /// <param name="objectId">Key</param>
        /// <returns></returns>
        public static T GetSingleCacheObject<T>(string objectId)
        {
            string redisKey = GetRedisKey<T>(objectId);

            using (PooledRedisClientManager.DisposablePooledClient<RedisClient> client = GetClient(redisKey))
            {
                if (client != null)
                {
                    object obj = null;

                    try
                    {
                        obj = client.Client.Get<T>(redisKey);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogTextException(ex);

                        throw;
                    }
                    finally
                    {
                    }

                    return (T)obj;
                }
                else
                {
                    return default(T);
                }


            }
        }

        /// <summary>
        /// 删除某个强类型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objecid"></param>
        /// <returns></returns>
        public static bool RemoveSingleCacheObject<T>(string objecid)
        {
            bool b = false;

            string redisKey = GetRedisKey<T>(objecid);

            using (PooledRedisClientManager.DisposablePooledClient<RedisClient> client = GetClient(redisKey))
            {
                if (client != null)
                {
                    b = client.Client.Remove(redisKey);

                    return b;
                }
                else
                {
                    return false;
                }
            }

            return b;
        }

        /// <summary>
        /// 判断某个强类型对象是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public static bool Exists<T>(string objectId)
        {
            string redisKey = GetRedisKey<T>(objectId);

            using (PooledRedisClientManager.DisposablePooledClient<RedisClient> client = GetClient(redisKey))
            {
                if (client != null)
                {
                    bool obj = false;

                    try
                    {
                        obj = client.Client.Exists(redisKey) > 0;
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogTextException(ex);
                    }

                    return obj;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 删除某个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objecid"></param>
        /// <returns></returns>
        public static bool RemoveSingleCache(string redisKey)
        {
            bool b = false;

            using (PooledRedisClientManager.DisposablePooledClient<RedisClient> client = GetClient(redisKey))
            {
                if (client != null)
                {
                    b = client.Client.Remove(redisKey);
                }
                else
                {
                    b = false;
                }
            }

            return b;
        }

        #endregion

        #region 存取一组强类型对象

        /// <summary>
        /// 将一组强类型对象存入某个键中
        /// </summary>
        /// <typeparam name="T">强类型</typeparam>
        /// <param name="listid">键</param>
        /// <param name="entities">一组强类型对象</param>
        public static void SetListCacheObject<T>(string listid, List<T> entities)
        {

            SetSingleCacheObject<List<T>>(listid, entities);

            //if (entities == null)
            //{
            //    return;
            //}

            //using (RedisClient client = GetClient())
            //{
            //    if (client != null)
            //    {
            //        //client.Set

            //        //Because IRedisClient do not provide a method that set alist to a cachelist,so add lock logic to confirm identity.

            //        if (redisobj.Count == 0)
            //        {
            //            lock (_LockPad)
            //            {
            //                if (redisobj.Count == 0)
            //                {
            //                    redisobj.Append(
            //                    redisobj.Add(entities);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            lock (_LockPad)
            //            {
            //                redisobj.Clear();

            //                redisobj.AddRange(entities);
            //            }
            //        }

            //        RedisManager.DisposeRedisClient(client);
            //    }
            //}

        }

        /// <summary>
        /// 从某个键中获取一组强类型对象
        /// </summary>
        /// <typeparam name="T">强类型</typeparam>
        /// <param name="listid">键</param>
        /// <returns></returns>
        public static List<T> GetListCacheObject<T>(string listid)
        {

            return GetSingleCacheObject<List<T>>(listid);
            //using (RedisClient client =GetClient())
            //{
            //    if (client != null)
            //    {
            //        IRedisTypedClient<T> redis = client.GetTypedClient<T>();
            //        var redisobj = redis.Lists[listid];
            //        return redisobj;
            //    }
            //    else
            //    {
            //        return default(IList<T>);
            //    }
            //}

        }

        //public static T GetSingleCacheOjbectFromList<T>(string listid, int index) 
        //{

        //    using (IRedisClient client =GetClient())
        //    {
        //        if (client != null)
        //        {
        //            IRedisTypedClient<T> redis = client.GetTypedClient<T>();
        //            var cacheObj = redis.GetItemFromList(redis.Lists[listid],index);                    
        //            return cacheObj;
        //        }
        //        else
        //        {
        //            return default(T);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 清除一组强类型缓存对象
        ///// </summary>
        ///// <typeparam name="T">类型</typeparam>
        ///// <param name="listid">键</param>
        //public static void ClearListCacheObject<T>(string listid)
        //{
        //    using (IRedisClient client = GetClient())
        //    {
        //        if (client != null)
        //        {
        //            IRedisTypedClient<T> redis = client.GetTypedClient<T>();
        //            redis.Lists[listid].Clear();
        //        }
        //    }

        //}

        ///// <summary>
        ///// 从一组强类型对象中移除某个索引的对象
        ///// </summary>
        ///// <typeparam name="T">类型</typeparam>
        ///// <param name="listid">键</param>
        ///// <param name="index">索引</param>
        //public static void RemoveSingleCacheObjectFromList<T>(string listid,int index)
        //{
        //    using (IRedisClient client = GetClient())
        //    {
        //        if (client != null)
        //        {
        //            IRedisTypedClient<T> redis = client.GetTypedClient<T>();
        //            redis.Lists[listid].RemoveAt(index);
        //        }
        //    }

        //}

        ///// <summary>
        ///// 从一组强类型对象中移除某个对象
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="listid">键</param>
        ///// <param name="obj">对象</param>
        //public static void RemoveSingleCacheObjectFromList<T>(string listid, T obj)
        //{
        //    using (IRedisClient client = GetClient())
        //    {
        //        if (client != null)
        //        {
        //            IRedisTypedClient<T> redis = client.GetTypedClient<T>();
        //            redis.Lists[listid].RemoveValue(obj);
        //        }
        //    }

        //}

        #endregion

        #region Hash

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetHashCacheObject<T, E>(string hashId, string key, E value)
        {
            string redisKey = GetRedisKey<T>(hashId);

            bool result = false;

            using (PooledRedisClientManager.DisposablePooledClient<RedisClient> client = GetClient(redisKey))
            {
                if (client != null)
                {
                    try
                    {
                        string serializedValue = TypeSerializer.SerializeToString<E>(value);
                        if (serializedValue != null)
                        {
                            //result = client.Client.HSet(redisKey, key, value);
                            result = client.Client.SetEntryInHash(redisKey, key, serializedValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogTextException(ex);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool DeleteHashCacheObject<T>(string hashId, string key)
        {
            string redisKey = GetRedisKey<T>(hashId);

            bool result = false;

            using (PooledRedisClientManager.DisposablePooledClient<RedisClient> client = GetClient(redisKey))
            {
                if (client != null)
                {
                    //result = client.Client.HDel(redisKey, key);
                    result = client.Client.RemoveEntryFromHash(redisKey, key);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static E GetHashCacheObject<T, E>(string hashId, string key)
        {
            string redisKey = GetRedisKey<T>(hashId);

            using (PooledRedisClientManager.DisposablePooledClient<RedisClient> client = GetClient(redisKey))
            {
                if (client != null)
                {
                    E obj = default(E);

                    try
                    {
                        string serializedValue = client.Client.GetValueFromHash(redisKey, key);
                        if (serializedValue != null)
                        {
                            //byte[] zippedObj = client.Client.HGet(redisKey, key);
                            obj = TypeSerializer.DeserializeFromString<E>(serializedValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogTextException(ex);
                    }

                    return (E)obj;
                }
            }

            return default(E);
        }

        #endregion

        #region 清空redis

        public static void FlushAll()
        {
            foreach (var item in RedisManager.GetAllClients())
            {
                item.FlushAll();
            }
        }

        public static void GetAllKeys()
        {
            foreach (var item in RedisManager.GetAllClients())
            {
                List<string> list = item.GetAllKeys();

                //LogHelper.LogTextException("");
            }
        }

        public static void Remove(params string[] keys)
        {
            foreach (var item in RedisManager.GetAllClients())
            {
                item.Del(keys);
            }
        }

        #endregion
    }
}
