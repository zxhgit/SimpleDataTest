using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zx.ApiContainer.Entity.Configration
{
    /// <summary>
    /// 数据语句执行参数
    /// </summary>
    public class SQLExcutiveCommand
    {
        /// <summary>
        /// SQL语句
        /// </summary>
        public string SQLText
        {
            get;
            set;
        }

        /// <summary>
        /// 参数词典
        /// </summary>
        public Dictionary<string, object> ParamDic
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public SQLExcutiveCommand()
        {
            ParamDic = new Dictionary<string, object>();
        }



    }
}
