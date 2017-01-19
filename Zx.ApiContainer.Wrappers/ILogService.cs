using System;

namespace Zx.ApiContainer.Wrappers
{
    /// <summary>
    /// 日志服务接口
    /// </summary>
    public interface ILogService : IBasicService
    {
        /// <summary>
        /// 发送异常至消息队列
        /// </summary>
        /// <param name="ex"></param>
        void SendExceptionLog(Exception ex);
    }
}
