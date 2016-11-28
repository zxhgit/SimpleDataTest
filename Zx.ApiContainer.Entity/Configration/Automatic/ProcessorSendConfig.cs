using System;
using Zx.ApiContainer.Entity.Attributes;

namespace Zx.ApiContainer.Entity.Configration
{
    /// <summary>
    /// 异步发送至消息队列配置
    /// </summary>
    [Serializable]
    public class ProcessorSendConfig
    {

        #region Properties

        /// <summary>
        /// 自增ID
        /// </summary>
        [PrimaryKeyAttribute]
        [IdentityAttribute]
        public int ID
        {
            get;
            set;
        }

        /// <summary>
        /// 实现类名称
        /// </summary>
        public string ImplementorClass
        {
            get;
            set;
        }

        /// <summary>
        /// 实现类程序集名称
        /// </summary>
        public string ImplementorProcessName
        {
            get;
            set;
        }

        /// <summary>
        /// 实现方法名称
        /// </summary>
        public string ImplementorMethodName
        {
            get;
            set;
        }


        /// <summary>
        /// 异步实现类的消息地址
        /// </summary>
        public string ImplementorMSMQPath
        {
            get;
            set;
        }

        /// <summary>
        /// 调用类型
        /// </summary>
        public short InvokerType
        {
            get;
            set;
        }

        /// <summary>
        /// QA消息队列地址
        /// </summary>
        public string ImplementorQAMSMQPath
        {
            get;
            set;
        }


        #endregion

    }
}
