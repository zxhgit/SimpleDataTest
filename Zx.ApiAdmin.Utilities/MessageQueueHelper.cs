using System.Messaging;
using System.Runtime.Serialization.Formatters;

namespace Zx.ApiAdmin.Utilities
{
    /// <summary>
    /// 消息队列处理帮助类
    /// </summary>
    public static class MessageQueueHelper
    {
        #region 发送消息队列

        /// <summary>
        /// 发送消息队列
        /// </summary>
        /// <param name="messageQueuePath">消息队列地址</param>
        /// <param name="sendObject">发送的实体</param>
        /// <returns></returns>
        public static void SendToQueue<T>(string messageQueuePath, T sendObject)
        {
            MessageQueue mq = new MessageQueue();

            mq.Path = messageQueuePath;

            //mq.Formatter = new BinaryMessageFormatter();
            mq.Formatter = new BinaryMessageFormatter(FormatterAssemblyStyle.Simple, FormatterTypeStyle.XsdString);
            //mq.Formatter =
            //    new XmlMessageFormatter(new[] {"ZhaoPin.HighEnd.Entities.Log.ExceptionEntity,ZhaoPin.HighEnd.Entity"});

            SendToQueue(mq, sendObject);
        }

        /// <summary>
        /// 获取消息队列对象
        /// </summary>
        /// <param name="messageQueuePath">消息队列地址</param>
        /// <returns></returns>
        public static MessageQueue GetMessageQueue(string messageQueuePath)
        {
            MessageQueue mq = new MessageQueue();

            mq.Path = messageQueuePath;

            mq.Formatter = new BinaryMessageFormatter();

            return mq;
        }

        /// <summary>
        /// 发送消息队列
        /// </summary>
        /// <param name="mq">消息队列</param>
        /// <param name="sendObject">实体</param>
        /// <returns></returns>
        public static void SendToQueue(MessageQueue mq, object sendObject)
        {
            mq.Send(sendObject);
        }

        #endregion
    }
}
