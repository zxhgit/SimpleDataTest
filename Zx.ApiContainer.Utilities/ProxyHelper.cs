using System;
using System.Collections.Generic;
using System.Reflection;

namespace Zx.ApiContainer.Utilities
{
    /// <summary>
    /// 定义接口/实现类的帮助方法，需自定义实现类的路径（"I"+接口名称=实现类的类名称）
    /// </summary>
    public class ProxyHelper
    {
        #region 属性

        /// <summary>
        /// 服务端实现类存储词典 key:接口类型FullName value：实现类构造函数
        /// </summary>
        static Dictionary<string, ConstructorInfo> _ServiceInstanceDic = new Dictionary<string, ConstructorInfo>();


        /// <summary>
        /// 单点锁定
        /// </summary>
        static object _LockPad = new object();

        #endregion

        #region （新增构造函数含有参数）

        /// <summary>
        /// 将一个新的配置加入缓存
        /// </summary>
        /// <param name="config">实现类配置</param>
        private static void InitServiceInstanceType(ImplementionConfigration config, bool? hasParam)
        {
            string key = config.InterfaceType.Name;

            string serviceName = key.Remove(0, 1);

            string servicePath = config.InstanceFrontPath + "." + serviceName + "," + config.InstanceProgrammer;

            Type serviceType = ReflectHelper.GetTypeByWholeUrl(servicePath);

            Type interfaceVerification = serviceType.GetInterface(key);

            if (interfaceVerification == null)
            {
                throw new Exception("指定类型的服务无法实现接口" + key);
            }

            ConstructorInfo constructor = null;

            if (hasParam.HasValue)
            {
                Type paramType = hasParam.Value.GetType();
                constructor = ReflectHelper.GetConstructor(servicePath, new Type[] { paramType });
            }
            else
            {
                constructor = ReflectHelper.GetDefaultConstructorInfo(serviceType);
            }

            _ServiceInstanceDic.Add(config.InterfaceType.FullName, constructor);
        }

        /// <summary>
        /// 将一个新的配置加入缓存
        /// </summary>
        /// <param name="config">实现类配置</param>
        private static void InitServiceInstance(ImplementionConfigration config, object[] paramsa)
        {
            string key = config.InterfaceType.Name;

            string serviceName = key.Remove(0, 1);

            string servicePath = config.InstanceFrontPath + "." + serviceName + "," + config.InstanceProgrammer;

            Type serviceType = ReflectHelper.GetTypeByWholeUrl(servicePath);

            Type interfaceVerification = serviceType.GetInterface(key);

            if (interfaceVerification == null)
            {
                throw new Exception("指定类型的服务无法实现接口" + key);
            }

            ConstructorInfo constructor = null;


            Type[] tArray = new Type[paramsa.Length];

            for (int i = 0; i < paramsa.Length; i++)
            {
                tArray[i] = paramsa[i].GetType();
            }

            constructor = ReflectHelper.GetConstructor(servicePath, tArray);

            _ServiceInstanceDic.Add(config.InterfaceType.FullName, constructor);
        }

        /// <summary>
        /// 通过接口及配置获取实现类
        /// </summary>
        /// <param name="config">接口及配置</param>
        /// <returns></returns>
        private static object GetServiceInstancePublic(ImplementionConfigration config, bool? hasParam)
        {
            object serviceInstance = null;

            if (config == null || config.InterfaceType == null)
            {
                throw new Exception("服务配置异常");
            }

            if (!_ServiceInstanceDic.ContainsKey(config.InterfaceType.FullName))
            {
                lock (_LockPad)
                {
                    if (!_ServiceInstanceDic.ContainsKey(config.InterfaceType.FullName))
                    {
                        InitServiceInstanceType(config, hasParam);
                    }
                }
            }

            ConstructorInfo constructor = _ServiceInstanceDic[config.InterfaceType.FullName];

            if (constructor == null)
            {
                throw new Exception("没有找到服务实例的构造函数");
            }

            if (hasParam.HasValue)
            {
                serviceInstance = constructor.Invoke(new object[] { hasParam.Value });
            }
            else
            {
                serviceInstance = constructor.Invoke(null);
            }

            return serviceInstance;

        }

        /// <summary>
        /// 通过接口及配置获取实现类
        /// </summary>
        /// <param name="config">接口及配置</param>
        /// <returns></returns>
        public static object GetServiceInstanceByParam(ImplementionConfigration config, object[] paramArray)
        {
            object serviceInstance = null;

            if (config == null || config.InterfaceType == null)
            {
                throw new Exception("服务配置异常");
            }

            if (!_ServiceInstanceDic.ContainsKey(config.InterfaceType.FullName))
            {
                lock (_LockPad)
                {
                    if (!_ServiceInstanceDic.ContainsKey(config.InterfaceType.FullName))
                    {
                        InitServiceInstance(config, paramArray);
                    }
                }
            }

            ConstructorInfo constructor = _ServiceInstanceDic[config.InterfaceType.FullName];

            if (constructor == null)
            {
                throw new Exception("没有找到服务实例的构造函数");
            }

            serviceInstance = constructor.Invoke(paramArray);

            return serviceInstance;

        }

        /// <summary>
        /// 通过接口及配置获取实现类
        /// </summary>
        /// <param name="config">接口及配置</param>
        /// <returns></returns>
        public static object GetServiceInstance(ImplementionConfigration config)
        {
            return GetServiceInstancePublic(config, null);
        }

        /// <summary>
        /// 通过接口及配置获取实现类
        /// </summary>
        /// <param name="config">接口及配置</param>
        /// <returns></returns>
        public static object GetServiceInstance(ImplementionConfigration config, bool isRead)
        {
            return GetServiceInstancePublic(config, isRead);
        }

        #endregion

        ///// <summary>
        ///// 将一个新的配置加入缓存
        ///// </summary>
        ///// <param name="config">实现类配置</param>
        //private static void InitServiceInstanceType(ImplementionConfigration config)
        //{

        //    string key = config.InterfaceType.Name;

        //    string serviceName = key.Remove(0, 1);

        //    string servicePath = config.InstanceFrontPath+"."+ serviceName + ","+config.InstanceProgrammer;

        //    Type serviceType = ReflectHelper.GetTypeByWholeUrl(servicePath);

        //    Type interfaceVerification = serviceType.GetInterface(key);

        //    if (interfaceVerification == null)
        //    {
        //        throw new Exception("指定类型的服务无法实现接口" + key);
        //    }

        //    ConstructorInfo constructor = ReflectHelper.GetDefaultConstructorInfo(serviceType);

        //    _ServiceInstanceDic.Add(config.InterfaceType.FullName, constructor);
        //}

        ///// <summary>
        ///// 通过接口及配置获取实现类
        ///// </summary>
        ///// <param name="config">接口及配置</param>
        ///// <returns></returns>
        //public static object GetServiceInstance(ImplementionConfigration config)
        //{
        //    object serviceInstance = null;

        //    if (config == null || config.InterfaceType == null)
        //    {
        //        throw new Exception("服务配置异常");
        //    }

        //    if (!_ServiceInstanceDic.ContainsKey(config.InterfaceType.FullName))
        //    {
        //        lock (_LockPad)
        //        {
        //            if (!_ServiceInstanceDic.ContainsKey(config.InterfaceType.FullName))
        //            {
        //                InitServiceInstanceType(config);
        //            }
        //        }
        //    }

        //    ConstructorInfo constructor = _ServiceInstanceDic[config.InterfaceType.FullName];

        //    if (constructor == null)
        //    {
        //        throw new Exception("没有找到服务实例的构造函数");
        //    }

        //    serviceInstance = constructor.Invoke(null);

        //    return serviceInstance;

        //}
    }

    /// <summary>
    /// 接口及实现类配置
    /// </summary>
    public class ImplementionConfigration
    {
        /// <summary>
        /// 接口类型
        /// </summary>
        public Type InterfaceType
        {
            get;
            set;
        }

        /// <summary>
        /// 实现实例前部
        /// </summary>
        public string InstanceFrontPath
        {
            get;
            set;
        }

        /// <summary>
        /// 实现实例程序集名
        /// </summary>
        public string InstanceProgrammer
        {
            get;
            set;
        }
    }
}
