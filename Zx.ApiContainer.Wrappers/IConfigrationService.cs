using System.Collections.Generic;
using Zx.ApiContainer.Entity.Configration;
using Zx.ApiContainer.Entity.Log.Automatic;
namespace Zx.ApiContainer.Wrappers
{
    /// <summary>
    /// 获取配置接口
    /// </summary>
    public interface IConfigrationService : IBasicService
    {
        /// <summary>
        /// 获取自定义配置
        /// </summary>
        /// <param name="appSettingKey"></param>
        /// <returns></returns>
        string GetAppSetting(string appSettingKey);

        /// <summary>
        /// 获取Redis地址配置信息
        /// </summary>
        /// <returns></returns>
        List<RedisConfig> GetRedisConfigLists();

        /// <summary>
        /// 获取异步处理器配置
        /// </summary>
        /// <param name="implementorMethodfullPath">实现方法全路径 类全路径.方法名,程序集名称</param>
        /// <returns></returns>
        ProcessorSendConfig GetProcessorSendConfig(string implementorMethodfullPath);
    }
}
