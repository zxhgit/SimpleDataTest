using System;
using System.Collections.Generic;

namespace Zx.ApiContainer.Entity.Configration
{
    /// <summary>
    /// 异步执行消息队列参数
    /// 一个异步操作为一个实例
    /// </summary>
    [Serializable]
    public class AsyncMethodParameter
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public AsyncMethodParameter()
        {
            this.ParameterTypes = new List<AsyncMethodParameterInfo>();
        }

        /// <summary>
        /// 方法名称
        /// </summary>
        public string MethodName
        {
            get;
            set;
        }

        /// <summary>
        /// 实现类名称
        /// </summary>
        public string ImplementorTypeFullName
        {
            get;
            set;
        }

        /// <summary>
        /// 应用程序集名称
        /// </summary>
        public string ImplementorAssembleyName
        {
            get;
            set;
        }

        /// <summary>
        /// 方法参数
        /// </summary>
        public object[] Parameters
        {
            get;
            set;
        }

        /// <summary>
        /// 参数配置
        /// </summary>
        public List<AsyncMethodParameterInfo> ParameterTypes
        {
            get;
            set;
        }

        /// <summary>
        /// 是否是泛型参数方法
        /// </summary>
        public bool IsGenericParaterMethod
        {
            get;
            set;
        }

        /// <summary>
        /// 泛型参数数量
        /// </summary>
        public int GenericParameterCount
        {
            get;
            set;
        }
    }
}
