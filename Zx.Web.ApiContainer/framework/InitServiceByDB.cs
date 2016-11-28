using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Zx.ApiContainer.Wrappers;
using Zx.Web.ApiContainer.model;

namespace Zx.Web.ApiContainer.framework
{
    public class InitServiceByDB : IInitService
    {
        private int GetSiteId()
        {
            var strSiteId = ConfigurationManager.AppSettings["SiteId"];
            int siteId;
            if (!int.TryParse(strSiteId, out siteId)) throw new InvalidOperationException("AppSettings-SiteId-类型异常");
            return siteId;
        }

        public List<RouteMapping> GetMappings()
        {
            var siteId = GetSiteId();
            var srv = ServiceFactory.GetSrv<IApiService>();
            var dbMappings = srv.GetMappings(siteId);
            var res = dbMappings.Select(dbm => new RouteMapping
            {
                Path = dbm.Path,
                Verb = dbm.Verb,
                ServiceAssembly = dbm.ServiceAssembly,
                ServiceName = dbm.ServiceName,
                MethodName = dbm.MethodName,
                IsAsyncInvoke = dbm.IsAsyncInvoke.GetValueOrDefault(),
                SiteId = dbm.SiteId
            }).ToList();
            return res;
        }
    }
}