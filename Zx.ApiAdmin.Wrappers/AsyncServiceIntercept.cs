using Castle.DynamicProxy;
using System;
using System.Reflection;
using Zx.ApiAdmin.Entity.Configration;
using Zx.ApiAdmin.Utilities;

namespace Zx.ApiAdmin.Wrappers
{
    /// <summary>
    /// 异步服务拦截
    /// </summary>
    public class AsyncServiceIntercept : StandardInterceptor
    {

        #region 拦截方法


        /// <summary>
        /// 方法拦截
        /// 异步操作发送至队列执行
        /// </summary>
        /// <param name="invocation">拦截配置</param>
        protected override void PerformProceed(IInvocation invocation)
        {
            //发送至消息队列的实体
            AsyncMethodParameter parameter = new AsyncMethodParameter();

            //方法名称
            parameter.MethodName = invocation.MethodInvocationTarget.Name;

            //实现类程序集名称
            parameter.ImplementorTypeFullName = invocation.TargetType.FullName;

            string assemblyFullName = invocation.TargetType.Assembly.FullName;

            parameter.ImplementorAssembleyName = ReflectHelper.FilterAssemblyName(assemblyFullName);

            string fullPath = parameter.ImplementorTypeFullName + "." + parameter.MethodName + "," + parameter.ImplementorAssembleyName;

            //实际方法
            MethodInfo method = invocation.MethodInvocationTarget;

            MethodInfo orginalMethod = method;

            //获取泛型方法
            if (method.IsGenericMethod)
            {
                orginalMethod = method.GetGenericMethodDefinition();

                parameter.IsGenericParaterMethod = true;
            }

            #region 参数设置

            //准备参数
            ParameterInfo[] methodParameters = orginalMethod.GetParameters();

            int genericParameterCount = 0;

            foreach (ParameterInfo parameterInfo in methodParameters)
            {
                AsyncMethodParameterInfo storeInfo = new AsyncMethodParameterInfo();

                //参数类型名称
                storeInfo.ParameterTypeName = parameterInfo.ParameterType.Name;

                //是否为泛型参数
                storeInfo.IsGenericParameter = parameterInfo.ParameterType.IsGenericParameter;

                //泛型参数数量
                if (parameterInfo.ParameterType.IsGenericParameter)
                {
                    genericParameterCount++;
                }

                parameter.ParameterTypes.Add(storeInfo);
            }

            //泛型参数数量
            parameter.GenericParameterCount = genericParameterCount;

            //实际参数存储
            parameter.Parameters = invocation.Arguments;

            #endregion

            string msmqPath = null;

            IConfigrationService configService = ServiceFactory.GetService<IConfigrationService>() as IConfigrationService;

            ProcessorSendConfig sendConfig = configService.GetProcessorSendConfig(fullPath);

            if (sendConfig == null)
            {
                throw new Exception("没有定义异步请求的配置:" + fullPath);
            }

            msmqPath = sendConfig.ImplementorMSMQPath;

            if (msmqPath == null)
            {
                throw new Exception("没有定义发送消息队列");
            }

            //发送至消息队列
            MessageQueueHelper.SendToQueue(msmqPath, parameter);

            //判断类型
            Type t = invocation.Method.ReturnType;

            //如果是bool型返回bool
            if (t == typeof(bool))
            {
                invocation.ReturnValue = true;
            }
            else
            {
                //Todo 其他值类型默认回参逻辑的确定
                if (t.IsValueType)
                {

                }
            }
        }

        #endregion
    }
}
