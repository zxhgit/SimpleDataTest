using System;
using System.Configuration;
namespace Zx.ApiAdmin.Wrappers
{
    /// <summary>
    /// 配置访问的帮助类
    /// </summary>
    public class ConfigrationHelper
    {
        /// <summary>
        /// 全局是否开启缓存的AppSettingKey
        /// </summary>
        const string GLOBAL_ENABLE_CACHE_APPSETTING_KEY = "GlobalEnableCache";
        /// <summary>
        /// 全局开启或关闭异步写入数据库标识
        /// </summary>
        const string Global_Enable_AsyncWriteToDB_AppSettingKey = "GloabalEnableAsyncWriteToDB";

        /// <summary>
        /// 获取服务类
        /// </summary>
        /// <returns></returns>
        public static IConfigrationService GetConfigrationService()
        {
            return (IConfigrationService)ServiceFactory.GetService<IConfigrationService>();
        }

        /// <summary>
        /// 是否全局启用缓存
        /// </summary>
        public static bool GlobalEnableCache
        {
            get
            {
                bool b = true;

                if (ConfigurationManager.AppSettings[GLOBAL_ENABLE_CACHE_APPSETTING_KEY] != null)
                {
                    b = Convert.ToBoolean(ConfigurationManager.AppSettings[GLOBAL_ENABLE_CACHE_APPSETTING_KEY]);
                }

                return b;
            }

        }

        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <param name="settingKey"></param>
        /// <returns></returns>
        public static string GetAppSetting(string settingKey)
        {
            if (settingKey != null && settingKey.ToLower() == "isenterpriseonline")
            {
                if (System.Configuration.ConfigurationManager.AppSettings["IsEnterpriseOnLine"] != null)
                {
                    return System.Configuration.ConfigurationManager.AppSettings["IsEnterpriseOnLine"];
                }
            }

            return GetConfigrationService().GetAppSetting(settingKey);
        }

        /// <summary>
        /// 是否全局启用异步
        /// </summary>
        public static bool GlobalEnableAsync
        {
            get
            {
                bool b = true;

                if (ConfigurationManager.AppSettings[Global_Enable_AsyncWriteToDB_AppSettingKey] != null)
                {
                    b = Convert.ToBoolean(ConfigurationManager.AppSettings[Global_Enable_AsyncWriteToDB_AppSettingKey]);
                }

                return b;
            }

        }

    }
}
