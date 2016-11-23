using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zx.ApiContainer.Wrappers.Cache
{
    /// <summary>
    /// Redis配置
    /// </summary>
    public sealed class RedisConfigInfo
    {


        /// <summary>
        /// Redis缓存时间
        /// </summary>
        public static TimeSpan HighEndRedisCacheTime
        {
            get
            {
                TimeSpan span = new TimeSpan(6048000000000);

                string values = ConfigrationHelper.GetAppSetting("HighEndRedisCacheTime");

                if (values != null)
                {
                    long intSpan = Convert.ToInt64(values);

                    span = new TimeSpan(intSpan);
                }

                return span;
            }
        }

        /// <summary>
        /// 高端项目RedisIP地址
        /// </summary>
        public static string HighEndRedisHost
        {
            get
            {
                string values = ConfigrationHelper.GetAppSetting("HighEndRedisHost");

                return values;
            }
        }

        /// <summary>
        /// 高端项目RedisIP地址
        /// </summary>
        public static int HighEndRedisPort
        {
            get
            {
                int port = 6379;

                string values = ConfigrationHelper.GetAppSetting("HighEndRedisPort");


                if (values != null)
                {
                    port = Convert.ToInt32(values);
                }

                return port;
            }
        }

    }
}
