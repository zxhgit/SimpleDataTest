using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zx.Web.ApiContainer.model;
//using ZhaoPin.HighEnd.Wrappers;
//using ZhaoPin.HighEnd.Entities.Configration;

namespace Zx.Web.ApiContainer.framework
{
    public class InitServiceByDll : IInitService
    {
        public List<model.RouteMapping> GetMappings()
        {
            //var srv = ServiceFactory.GetSrv<IConfigrationService>();
            //List<Entities.Configration.RouteMapping> list = srv.GetSiteRouteMappings(3);
            //var res = list.Select(r => new model.RouteMapping
            //{
            //    Path = r.Path,
            //    Verb = r.Verb,
            //    ServiceAssembly = r.ServiceAssembly,
            //    ServiceName = r.ServiceName,
            //    MethodName = r.MethodName,
            //    IsAsyncInvoke = r.IsAsyncInvoke.GetValueOrDefault(),
            //    SiteId = r.SiteId,
            //    BusinessName = r.BusinessName
            //}).ToList();
            //return res;
            return null;
        }
    }
}