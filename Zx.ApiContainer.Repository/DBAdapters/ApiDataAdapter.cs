using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zx.ApiContainer.Services.DataAdapters;

namespace Zx.ApiContainer.Repository.DBAdapters
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
    }
}
