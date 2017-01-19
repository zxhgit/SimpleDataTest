using System;

namespace Zx.ApiContainer.Entity.Configration
{
    /// <summary>
    /// 异步执行方法的参数配置
    /// 存储参数类型的名称，是否为泛型参数
    /// </summary>
    [Serializable]
    public class AsyncMethodParameterInfo
    {
        /// <summary>
        /// 参数类型名称
        /// </summary>
        public string ParameterTypeName
        {
            get;
            set;
        }

        /// <summary>
        /// 是否是泛型参数
        /// </summary>
        public bool IsGenericParameter
        {
            get;
            set;
        }
    }
}
