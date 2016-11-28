using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zx.ApiAdmin.Entity.Admin;

namespace Zx.Web.ApiAdmin.Models.ApiContainer
{
    public class ClassMethodModel
    {
        /// <summary>
        /// 站点id
        /// </summary>
        public int SiteId { get; set; }
        /// <summary>
        /// dll文件名
        /// </summary>
        public string Dname { get; set; }
        /// <summary>
        /// 类名
        /// </summary>
        public string Cname { get; set; }

        internal List<ApiContainerMethodInfo> MethodsInDll { get; set; }

        internal List<ApiContainerRouteMapping> MethodsInMapping { get; set; }

        /// <summary>
        /// 获取带有比较效果的结果（dll中与已有mapping中）
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<object> GetResWithCompare()
        {
            //dll中methods与数据库mapping中的method做leftjoin，根据mapping中的值设置默认选中项
            var res = from dm in MethodsInDll
                      join tm in MethodsInMapping on dm.Name equals tm.MethodName into joined
                      from jm in joined.DefaultIfEmpty()
                      let jmkey = dm
                      group jm by jmkey
                          into gm
                          let aggregate = gm.FirstOrDefault()
                          select new
                          {
                              Class = Cname ?? string.Empty,
                              gm.Key.Name,
                              gm.Key.ReturnTypeName,
                              ParmType = String.Join(",", gm.Key.ParmTypes.Select(p => p.Item2).ToArray()),
                              IsStatic = gm.Key.IsStatic.ToString(),
                              IsAppeared = aggregate != null
                          };
            return res;
        }
    }
}