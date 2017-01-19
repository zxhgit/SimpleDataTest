using System;
using System.Collections.Generic;
using System.Reflection;
using Zx.Web.ApiContainer.model;

namespace Zx.Web.ApiContainer.core
{
    /// <summary>
    /// ServiceDescriptor
    /// </summary>
    /// <remarks>不构建从Service到Method的引用（Service包含Method集合），Method的分组方式不是按Service（可根据site职责灵活配置）；
    /// 构建从Method到Service的引用，Method利用Service实例进行Invoke</remarks>
    public class ServiceDescriptor
    {
        //private static readonly Assembly ServiceAssemblies = Assembly.LoadFrom(EntryModule.LibPath + "ZhaoPin.HighEnd.Services.dll");
        private static readonly Dictionary<string, ServiceDescriptor> DicServiceDecs = new Dictionary<string, ServiceDescriptor>();
        private static readonly object LockPad = new object();
        private readonly string _serviceName;


        public readonly object ServiceInstance;//考虑service单例是否有问题
        public readonly Type ServiceType;
        public string ServiceName
        {
            get
            {
                return _serviceName;
            }
        }

        public static ServiceDescriptor GetDescriptorInstance(RouteMapping routeMapping)//application启动时调用，action进入时调用
        {
            if (!DicServiceDecs.ContainsKey(routeMapping.ServiceName))
            {
                lock (LockPad)
                {
                    if (!DicServiceDecs.ContainsKey(routeMapping.ServiceName))
                    {
                        var dec = new ServiceDescriptor(routeMapping);
                        DicServiceDecs.Add(routeMapping.ServiceName, dec);
                    }
                }
            }
            return DicServiceDecs[routeMapping.ServiceName];
        }

        private ServiceDescriptor(RouteMapping routeMapping)
        {
            _serviceName = routeMapping.ServiceName;
            var assemblies = Assembly.LoadFrom(GetAssembliesFile(routeMapping));//业务文件夹、版本文件夹            
            ServiceType = assemblies.GetType(_serviceName);//ServiceType==null 验证 
            try
            {
                ServiceInstance = Activator.CreateInstance(ServiceType, false); //ServiceInstance 验证
            }
            catch (Exception e)
            {
                ServiceInstance = null;
            }

        }

        private string GetAssembliesFile(RouteMapping routeMapping)
        {
            //业务文件夹、版本文件夹
            var res = EntryModule.LibPath + "/" + routeMapping.ServiceAssembly;
            return res;
        }
    }
}