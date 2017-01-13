using System;
using System.Reflection;
namespace Zx.Web.ApiContainer.core
{
    public class ParameterDescriptor
    {
        public Type ParameterType
        {
            get { return ParameterInfo.ParameterType; }
        }
        public string ParameterName
        {
            get { return ParameterInfo.Name; }
        }
        public ParameterInfo ParameterInfo { get; private set; }
        /// <summary>
        /// 参数默认值
        /// </summary>
        /// <remarks>绑定后为null时（数据源未指定）采用该值</remarks>
        public object DefaultValue
        {
            get { return ParameterInfo.DefaultValue; }
        }

        //binder对象

        public ParameterDescriptor(ParameterInfo parameterInfo)
        {
            ParameterInfo = parameterInfo;
        }
    }
}