using System;
using System.Collections.Generic;
using Zx.ApiAdmin.Entity.Configration;
using Zx.ApiAdmin.Entity.Log.Automatic;
using Zx.ApiAdmin.Services.DataAdapters;
using Zx.ApiAdmin.Wrappers;

namespace Zx.ApiAdmin.Services
{
    /// <summary>
    /// 配置服务类
    /// </summary>
    public class ConfigrationService : IConfigrationService
    {
        /// <summary>
        /// 锁定AppSetting
        /// </summary>
        static object _LockAppSetting = new object();

        /// <summary>
        /// 单点锁定异步处理器配置
        /// </summary>
        private static object _LockPad = new object();

        /// <summary>
        /// 异步处理器的词典
        /// </summary>
        private static Dictionary<string, ProcessorSendConfig> _ProcessorSendDic = null;

        /// <summary>
        /// AppSetting的缓存配置
        /// </summary>
        static Dictionary<string, AppSetting> _AppSettingDic = new Dictionary<string, AppSetting>();

        #region 数据访问适配器

        /// <summary>
        /// 获取数据访问适配器
        /// </summary>
        /// <returns></returns>
        private IConfigrationDataAdapter GetAdapter()
        {
            return DataAdapterFactory.GetConfigrationDataAdapter();
        }

        #endregion


        /// <summary>
        /// 获取配置节
        /// </summary>
        /// <param name="appSettingKey">配置节Key</param>
        /// <returns></returns>
        public string GetAppSetting(string appSettingKey)
        {
            string values = null;

            if (appSettingKey == null || appSettingKey == string.Empty)
            {
                return values;
            }

            string keys = appSettingKey.ToLower();

            if (!_AppSettingDic.ContainsKey(keys))
            {
                lock (_LockAppSetting)
                {
                    if (!_AppSettingDic.ContainsKey(keys))
                    {
                        AppSetting appSetting = this.GetAdapter().QueryEntireFromDB<AppSetting>(appSettingKey);

                        if (appSetting != null)
                        {
                            _AppSettingDic.Add(keys, appSetting);
                        }
                    }
                }
            }

            if (!_AppSettingDic.ContainsKey(keys))
            {
                throw new Exception("没有配置关键词" + keys);
            }

            AppSetting setting = _AppSettingDic[keys];

            if (setting != null)
            {
                values = setting.AppSettingValue;
            }

            return values;
        }

        /// <summary>
        /// 获取Redis地址配置信息
        /// </summary>
        /// <returns></returns>
        public List<RedisConfig> GetRedisConfigLists()
        {
            List<RedisConfig> list = this.GetAdapter().Query<RedisConfig>("1=1", null);

            return list;
        }

        /// <summary>
        /// 获取异步处理器配置
        /// </summary>
        /// <param name="implementorMethodfullPath">实现方法全路径 类全路径.方法名,程序集名称</param>
        /// <returns></returns>
        public ProcessorSendConfig GetProcessorSendConfig(string implementorMethodfullPath)
        {
            ProcessorSendConfig config = null;

            InitProcessorSendDic();

            if (_ProcessorSendDic.ContainsKey(implementorMethodfullPath.ToLower()))
            {
                config = _ProcessorSendDic[implementorMethodfullPath.ToLower()];
            }

            return config;
        }

        /// <summary>
        /// 加载发送至消息队列处理的配置信息
        /// </summary>
        private void InitProcessorSendDic()
        {
            if (_ProcessorSendDic == null)
            {
                lock (_LockPad)
                {
                    if (_ProcessorSendDic == null)
                    {
                        Dictionary<string, ProcessorSendConfig> dic = new Dictionary<string, ProcessorSendConfig>();

                        List<ProcessorSendConfig> list = this.GetAdapter().Query<ProcessorSendConfig>("1=1", null);

                        if (list != null && list.Count > 0)
                        {
                            foreach (ProcessorSendConfig config in list)
                            {
                                string serviceFullPath = config.ImplementorClass.ToLower() + "." + config.ImplementorMethodName.ToLower() + "," + config.ImplementorProcessName.ToLower();

                                dic.Add(serviceFullPath, config);
                            }
                        }

                        _ProcessorSendDic = dic;
                    }
                }

            }
        }

    }
}
