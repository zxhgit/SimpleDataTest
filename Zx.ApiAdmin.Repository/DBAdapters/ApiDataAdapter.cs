using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zx.ApiAdmin.Services.DataAdapters;
using Zx.ApiAdmin.Utilities.Data;

namespace Zx.ApiAdmin.Repository.DBAdapters
{
    public class ApiDataAdapter : BasicDBAdapter, IApiDataAdapter
    {
        /// <summary>
        /// 当前DataAdapter使用的数据库
        /// </summary>
        const string DB_KEY = "Zx.ApiContainer";

        /// <summary>
        /// 初始化
        /// </summary>
        public ApiDataAdapter(bool isRead = false)
            : base(isRead)
        {
            this.DBKey = DB_KEY;
        }

        /// <summary>
        /// 删除指定api站点的RouteMapping
        /// </summary>
        /// <param name="siteId"></param>
        public void DeleteApiContainerRouteMapping(int siteId)
        {
            const string sqltxt = @"delete ApiContainerRouteMapping where SiteId=@SiteId";
            var paramDic = new Dictionary<string, object> { { "@SiteId", siteId } };
            DBAccessor.ExecuteNonQuery(DBKey, sqltxt, paramDic);
        }
    }
}
