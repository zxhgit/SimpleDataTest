using System;
using Zx.ApiAdmin.Utilities;
using Zx.ApiAdmin.Wrappers;

namespace Zx.ApiAdmin.Services.DataAdapters
{
    public class DataAdapterFactory
    {
        /// <summary>
        /// 实现类的前部
        /// </summary>
        //const string ServiceInstanceFrontPath = "ZhaoPin.HighEnd.Repositories.DBAdapters";
        //private const string ServiceInstanceFrontPath = "ZhaoPin.HighEnd.ApiContainer.Repository.DBAdapters";
        private const string ServiceInstanceFrontPath = "Zx.ApiAdmin.Repository.DBAdapters";

        /// <summary>
        /// 实现类的程序集名称
        /// </summary>
        //const string ServiceProgrammer = "ZhaoPin.HighEnd.Repositories";
        //const string ServiceProgrammer = "ZhaoPin.HighEnd.ApiContainer.Repository";
        const string ServiceProgrammer = "Zx.ApiAdmin.Repository";

        /// <summary>
        /// 获取服务实现实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="asynchronous">是否为异步操作</param>
        /// <param name="isRead">构造函数参数</param>
        /// <returns></returns>
        public static IBasicDataAdapter GetDataAdapter<T>(bool asynchronous, bool isRead)
        {
            return GetDataAdapterPublic<T>(asynchronous, isRead);
        }

        /// <summary>
        /// 获取服务实现实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="asynchronous">是否为异步操作</param>
        /// <param name="hasParam">构造函数参数</param>
        /// <returns></returns>
        private static IBasicDataAdapter GetDataAdapterPublic<T>(bool asynchronous, bool? hasParam)
        {
            Type interfaceType = typeof(T);

            ImplementionConfigration config = new ImplementionConfigration();

            config.InterfaceType = interfaceType;


            config.InstanceFrontPath = ServiceInstanceFrontPath;

            config.InstanceProgrammer = ServiceProgrammer;

            object serviceInstance = null;

            if (hasParam.HasValue)
            {
                serviceInstance = ProxyHelper.GetServiceInstance(config, hasParam.Value);
            }
            else
            {
                serviceInstance = ProxyHelper.GetServiceInstance(config);
            }

            IBasicDataAdapter adapter = (IBasicDataAdapter)serviceInstance;

            adapter.EnableCache = ConfigrationHelper.GlobalEnableCache;

            return adapter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IConfigrationDataAdapter GetConfigrationDataAdapter(bool asynchronous, bool isRead = false)
        {
            return (IConfigrationDataAdapter)GetDataAdapter<IConfigrationDataAdapter>(asynchronous, isRead);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IConfigrationDataAdapter GetConfigrationDataAdapter(bool isRead = false)
        {
            return GetConfigrationDataAdapter(false, isRead);
        }

        public static IApiDataAdapter GetApiDataAdapter(bool isRead = false)
        {
            return (IApiDataAdapter)GetDataAdapter<IApiDataAdapter>(false, isRead);
        }
    }
}
