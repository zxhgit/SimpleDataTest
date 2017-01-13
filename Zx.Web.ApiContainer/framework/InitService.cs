using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using Zx.Web.ApiContainer.external;
using Zx.Web.ApiContainer.model;

namespace Zx.Web.ApiContainer.framework
{
    public class InitService : IInitService
    {
        private static readonly string LibPath = HttpContext.Current.Server.MapPath("/") + "/corelib/";
        //private const string DefaultEntitiesAssembley = "ZhaoPin.HighEnd.Entities.dll";
        //private const string MappingEntityType = "ZhaoPin.HighEnd.Entities.Configration.ApiContainerRouteMapping";
        //private const string ServiceAssembley = "ZhaoPin.HighEnd.Services.dll";
        //private const string ServiceName = "ZhaoPin.HighEnd.Services.ConfigrationService";
        private const string DefaultEntitiesAssembley = "Zx.Entities.dll";
        private const string MappingEntityType = "Zx.Entities.Configration.ApiContainerRouteMapping";
        private const string ServiceAssembley = "Zx.Services.dll";
        private const string ServiceName = "Zx.Services.ConfigrationService";
        private const string MethodName = "GetApiContainerRouteMappingBySite";


        private int GetSiteId()
        {
            var strSiteId = ConfigurationManager.AppSettings["SiteId"];
            int siteId;
            if (!int.TryParse(strSiteId, out siteId)) throw new InvalidOperationException("AppSettings-SiteId-类型异常");
            return siteId;
        }

        public List<RouteMapping> GetMappings()
        {
            var entityAssemblies = Assembly.LoadFrom(LibPath + DefaultEntitiesAssembley);
            var retValType = entityAssemblies.GetType(MappingEntityType);
            var assemblies = Assembly.LoadFrom(LibPath + ServiceAssembley);
            var serviceType = assemblies.GetType(ServiceName);
            var instance = Activator.CreateInstance(serviceType, false);
            var methodInfo = serviceType.GetMethod(MethodName, new[] { typeof(int) });
            var siteId = GetSiteId();
            var retVal = methodInfo.Invoke(instance, new object[] { siteId });
            var retValIEnumerable = retVal as IEnumerable<object>;
            if (retValIEnumerable == null) throw new InvalidOperationException("RouteMapping-返回异常");
            var res = new List<RouteMapping>();
            retValIEnumerable.ToList().ForEach(o =>
            {
                var mapping = BuildMapping(retValType, o);
                res.Add(mapping);
            });
            return res;
        }

        private RouteMapping BuildMapping(Type type, object instance)
        {
            const BindingFlags propertyFlags = BindingFlags.Public | BindingFlags.Instance;
            var routeMapping = new RouteMapping();
            var pTuples =
                from outer in type.GetProperties(propertyFlags)
                join inner in typeof(RouteMapping).GetProperties(propertyFlags) on outer.Name equals inner.Name
                select new Tuple<PropertyInfo, PropertyInfo>(outer, inner);
            pTuples.ToList().ForEach(tuple => tuple.Item2.SetValue(routeMapping, tuple.Item1.GetValue(instance)));
            return routeMapping;
        }
    }
}