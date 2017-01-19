using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zx.ApiAdmin.Entity.Admin;

namespace Zx.Web.ApiAdmin.Models.ApiContainer
{
    public class ClientMappingModel
    {
        public int SiteId { get; set; }
        public List<ClientMappingItemModel> Mappings { get; set; }

        internal List<ApiContainerRouteMapping> AppearedMappings { get; set; }

        /// <summary>
        /// 获取带有默认设置的结果
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<object> GetResWithDefaultSetting()
        {
            //post中的mapping与数据库mapping做leftjoin，用于显示本次新增项
            var res =
                (from pm in Mappings
                 join am in AppearedMappings
                     on pm.ServiceName + pm.MethodName equals am.ServiceName + am.MethodName into joinedM
                 from jm in joinedM.DefaultIfEmpty()
                 let jmkey = new Tuple<string, string, string>(pm.ServiceAssembly, pm.ServiceName, pm.MethodName)
                 group jm by jmkey
                     into gm
                     let firstOrDefault = gm.FirstOrDefault()
                     select new
                     {
                         ServiceAssembly = gm.Key.Item1,
                         ServiceName = gm.Key.Item2,
                         MethodName = gm.Key.Item3,
                         Path = firstOrDefault == null ? string.Empty : firstOrDefault.Path,
                         Verb = firstOrDefault == null ? string.Empty : firstOrDefault.Verb,
                         IsAsync = firstOrDefault == null || firstOrDefault.IsAsyncInvoke.GetValueOrDefault(),
                         State = firstOrDefault == null ? "add" : "nochange"
                     }).ToList();
            //数据库mapping与上一步结果做leftjoin，用于显示本次删除项
            var delete =
                (from bm in AppearedMappings
                 join cm in res
                     on bm.ServiceName + bm.MethodName equals cm.ServiceName + cm.MethodName into joinedM
                 from jm in joinedM.DefaultIfEmpty()
                 let jmkey = bm
                 group jm by jmkey
                     into gm
                     let rightlist = gm.FirstOrDefault()
                     where rightlist == null
                     select new
                     {
                         gm.Key.ServiceAssembly,
                         gm.Key.ServiceName,
                         gm.Key.MethodName,
                         gm.Key.Path,
                         gm.Key.Verb,
                         IsAsync = gm.Key.IsAsyncInvoke.GetValueOrDefault(),
                         State = "delete"
                     }).ToList();
            res.AddRange(delete);
            var finalRes = res.OrderBy(m => m.ServiceName + m.MethodName).ToList();
            return finalRes;
        }
    }

    public class ClientMappingItemModel
    {
        public string ServiceAssembly { get; set; }
        public string ServiceName { get; set; }
        public string MethodName { get; set; }

        public string Verb { get; set; }
        public string Path { get; set; }
        public bool IsAsync { get; set; }
    }
}