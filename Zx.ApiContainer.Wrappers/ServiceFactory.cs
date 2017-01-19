using Castle.DynamicProxy;
using System;
using Zx.ApiContainer.Utilities;

namespace Zx.ApiContainer.Wrappers
{
    /// <summary>
    /// 服务端实例实现类
    /// </summary>
    public class ServiceFactory
    {

        #region Constants

        /// <summary>
        /// 实现类的前部
        /// </summary>
        //const string ServiceInstanceFrontPath = "ZhaoPin.HighEnd.Services";
        //private const string ServiceInstanceFrontPath = "ZhaoPin.HighEnd.ApiContainer.Services";
        private const string ServiceInstanceFrontPath = "Zx.ApiContainer.Services";

        /// <summary>
        /// 实现类的程序集名称
        /// </summary>
        //const string ServiceProgrammer = "ZhaoPin.HighEnd.Services";
        //const string ServiceProgrammer = "ZhaoPin.HighEnd.ApiContainer.Services";
        const string ServiceProgrammer = "Zx.ApiContainer.Services";

        #endregion


        #region 主要逻辑
        /// <summary>
        /// 获取服务实现实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public static T GetSrv<T>() where T : IBasicService
        {
            return (T)GetService<T>();
        }
        /// <summary>
        /// 获取服务实现实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public static IBasicService GetService<T>()
        {
            Type interfaceType = typeof(T);

            return GetService(interfaceType);
        }

        public static IBasicService GetService(Type type)
        {
            ImplementionConfigration config = new ImplementionConfigration();

            config.InterfaceType = type;

            config.InstanceFrontPath = ServiceInstanceFrontPath;

            config.InstanceProgrammer = ServiceProgrammer;

            object serviceInstance = ProxyHelper.GetServiceInstance(config);

            return (IBasicService)serviceInstance;
        }

        /// <summary>
        /// 获取服务实现实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="asynchronous">是否为异步操作</param>
        /// <returns></returns>
        public static IBasicService GetService<T>(bool asynchronous)
        {

            if (!ConfigrationHelper.GlobalEnableAsync)
            {
                return GetService<T>();
            }

            Type interfaceType = typeof(T);

            ImplementionConfigration config = new ImplementionConfigration();

            config.InterfaceType = interfaceType;

            config.InstanceFrontPath = ServiceInstanceFrontPath;

            config.InstanceProgrammer = ServiceProgrammer;

            object serviceInstance = ProxyHelper.GetServiceInstance(config);

            return GetAsynGetService(config);
        }

        /// <summary>
        /// 获取异步数据操作代理
        /// </summary>
        /// <returns></returns>
        private static IBasicService GetAsynGetService(ImplementionConfigration config)
        {

            #region Emit异步代理

            ProxyGenerator pg = new ProxyGenerator();

            IInterceptor isp = new AsyncServiceIntercept();

            string key = config.InterfaceType.Name;

            string serviceName = key.Remove(0, 1);

            string fullPath = config.InstanceFrontPath + "." + serviceName + "," + config.InstanceProgrammer;

            string servicePath = fullPath;

            object serviceInstance = ReflectHelper.CreateInstanceByWholeUrl(servicePath);

            object dynamicProxy = pg.CreateInterfaceProxyWithTarget(config.InterfaceType, serviceInstance, isp);

            #endregion

            return (IBasicService)dynamicProxy;
        }


        #endregion
    }
}
