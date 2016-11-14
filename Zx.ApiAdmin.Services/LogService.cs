using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Zx.ApiAdmin.Entity.Configration;
using Zx.ApiAdmin.Entity.Log;
using Zx.ApiAdmin.Utilities;
using Zx.ApiAdmin.Wrappers;
//using ZhaoPin.HighEnd.Entities.Log;

namespace Zx.ApiAdmin.Services
{
    public class LogService : ILogService
    {
        /// <summary>
        /// 发送异常至消息队列
        /// </summary>
        /// <param name="ex"></param>
        public void SendExceptionLog(Exception ex)
        {
            string fullPath = "Zx.TaskSystem.ApiExceptionMessageProcessor.ApiExceptionMessageProcessor,Zx.TaskSystem";

            IConfigrationService configService = ServiceFactory.GetService<IConfigrationService>() as IConfigrationService;

            ProcessorSendConfig sendConfig = configService.GetProcessorSendConfig(fullPath);

            if (sendConfig == null)
            {
                throw new Exception("没有定义异步请求的配置:" + fullPath);
            }

            string msmqPath = sendConfig.ImplementorMSMQPath;

            if (msmqPath == null)
            {
                throw new Exception("没有定义发送消息队列");
            }

            ExceptionEntity entity = new ExceptionEntity();

            entity.HostName = Dns.GetHostName();

            entity.CurrentException = ex;

            var ent = new
            {
                HostName = Dns.GetHostName(),
                CurrentException = ex
            };

            try
            {
                //发送至消息队列
                MessageQueueHelper.SendToQueue<ExceptionEntity>(msmqPath, entity);
                //MessageQueueHelper.SendToQueue<object>(msmqPath, ent);
            }
            catch
            {
                LogHelper.LogTextException(ex);
            }

        }
    }
}
