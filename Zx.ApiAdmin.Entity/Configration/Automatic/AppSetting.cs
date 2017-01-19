using System;
using Zx.ApiAdmin.Entity.Attributes;

namespace Zx.ApiAdmin.Entity.Configration
{
    /// <summary>
    /// 全局配置
    /// </summary>
    [Serializable]
    public class AppSetting
    {

        #region Properties

        /// <summary>
        /// AppSetting的key值
        /// </summary>
        [PrimaryKeyAttribute]
        public string AppSettingKey
        {
            get;
            set;
        }

        /// <summary>
        /// 数据值
        /// </summary>
        public string AppSettingValue
        {
            get;
            set;
        }

        /// <summary>
        /// 应用程序域标识
        /// </summary>
        public string DomainKey
        {
            get;
            set;
        }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description
        {
            get;
            set;
        }


        #endregion

    }
}
