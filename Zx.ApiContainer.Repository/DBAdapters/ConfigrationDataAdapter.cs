using System;
using System.Collections.Generic;
using Zx.ApiContainer.Services.DataAdapters;
using Zx.ApiContainer.Utilities.Data;

namespace Zx.ApiContainer.Repository.DBAdapters
{
    /// <summary>
    /// 配置的数据库
    /// </summary>
    public class ConfigrationDataAdapter : BasicDBAdapter, IConfigrationDataAdapter
    {
        /// <summary>
        /// 
        /// </summary>
        const string DB_KEY = "ZhaoPin.HighEnd.ConfigDB";

        /// <summary>
        /// 初始化
        /// </summary>
        public ConfigrationDataAdapter(bool isRead = false)
            : base(isRead)
        {
            this.DBKey = DB_KEY;
        }

        /// <summary>
        /// 修改方法
        /// </summary>
        /// <param name="sqltxt"></param>
        /// <param name="paramDic"></param>
        /// <returns></returns>
        public bool UpdateObjMessage(string sqltxt, Dictionary<string, object> paramDic)
        {
            return DBAccessor.ExecuteNonQuery(this.DBKey, sqltxt, paramDic) > 0 ? true : false;
        }

        /// <summary>
        /// 获取智联加密字符串
        /// </summary>
        /// <param name="sourcePassword">源字符串</param>
        /// <returns></returns>
        public string GetZhaoPinEncryptedString(string sourcePassword)
        {
            string encryptedString = null;

            string sql = "select dbo.FN_SHA1(@SourcePassword)";

            Dictionary<string, object> paramDic = new Dictionary<string, object>();

            paramDic.Add("@SourcePassword", sourcePassword);

            object result = DBAccessor.ExecuteScalar(this.DBKey_ReadOnly, sql, paramDic);

            if (result != null)
            {
                if (!(result is DBNull))
                {
                    encryptedString = Convert.ToString(result);
                }
            }

            return encryptedString;
        }
        /// <summary>
        /// 通过类别Id和序号获取键值
        /// </summary>
        /// <param name="CategoryID"></param>
        /// <param name="OrderNumber"></param>
        /// <returns></returns>
        public int GetMaxDicItemValue(int CategoryID, int OrderNumber)
        {
            string sql = "select DicItemValue from TaxonomyData where CategoryID=@CategoryID and OrderNumber=@OrderNumber";
            Dictionary<string, object> paramDic = new Dictionary<string, object>();

            paramDic.Add("@CategoryID", CategoryID);
            paramDic.Add("@OrderNumber", OrderNumber);
            object result = DBAccessor.ExecuteScalar(this.DBKey_ReadOnly, sql, paramDic);
            int iResult = 0;
            if (result != null)
            {
                if (!(result is DBNull))
                {
                    iResult = Convert.ToInt32(result);
                }
            }
            return iResult;
        }





    }
}
