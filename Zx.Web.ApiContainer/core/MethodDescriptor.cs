using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web.UI;
using Nancy.Routing;
using Zx.Web.ApiContainer.ErrorHandler;
using Zx.Web.ApiContainer.external;
using Zx.Web.ApiContainer.model;
using System.Threading.Tasks;

namespace Zx.Web.ApiContainer.core
{
    public class MethodDescriptor
    {
        /// <summary>
        /// MethodDescriptor集合
        /// </summary>
        /// <remarks>
        /// servicename,methodname联合做未key
        /// </remarks>
        private static readonly Dictionary<Tuple<string, string>, MethodDescriptor> DicMethodDec =
            new Dictionary<Tuple<string, string>, MethodDescriptor>();
        private static readonly object LockPad = new object();

        private ParameterDescriptor[] _parametersCache;

        public readonly ServiceDescriptor ServiceDescriptor;
        public readonly string MethodName;
        public readonly MethodInfo MethodInfo;
        public readonly bool IsAsyncInvoke;

        public static MethodDescriptor GetDescriptorInstance(RouteMapping routeMapping)
        {
            //有namespace不用再加assemblyname
            var tupe = new Tuple<string, string>(routeMapping.ServiceName, routeMapping.MethodName);
            if (!DicMethodDec.ContainsKey(tupe))
            {
                lock (LockPad)
                {
                    if (!DicMethodDec.ContainsKey(tupe))
                    {
                        var dec = new MethodDescriptor(routeMapping);
                        DicMethodDec.Add(tupe, dec);
                    }
                }
            }
            return DicMethodDec[tupe];
        }

        private MethodDescriptor(RouteMapping routeMapping)
        {
            MethodName = routeMapping.MethodName;
            ServiceDescriptor = ServiceDescriptor.GetDescriptorInstance(routeMapping);
            IsAsyncInvoke = routeMapping.IsAsyncInvoke;
            MethodInfo = ServiceDescriptor.ServiceType.GetMethod(routeMapping.MethodName);//考虑重载
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 不同于DicMethodDec，对_parametersCache的操作是原子操作，针对整个数组而不是数组元素
        /// 而DicMethodDec则可能会有多个线程分别对其中加入元素，或读取其中不同元素，所以两者线程同步方式不一样
        /// </remarks>
        public ParameterDescriptor[] GetParameterDescriptor()
        {
            //原子操作
            var existingCache = Interlocked.CompareExchange(ref _parametersCache, null, null);
            if (existingCache != null) return existingCache;
            var pInfos = MethodInfo.GetParameters();
            var pdList = new List<ParameterDescriptor>(pInfos.Length);
            //mvc源码建议用arrays且不用linq
            for (int i = 0; i < pInfos.Length; i++)
            {
                pdList.Add(new ParameterDescriptor(pInfos[i]));
            }
            var pds = pdList.ToArray();
            //cache为null则用pds更新cache
            var updatedCache = Interlocked.CompareExchange(ref _parametersCache, pds, null);
            return (ParameterDescriptor[])(updatedCache ?? pds).Clone();
        }

        public object Execute(IDictionary<string, object> parameters)
        {
            var invokeRes = MethodInfo.Invoke(BuildInvokeInstance(), BuildInvokeParms(parameters));
            return invokeRes;
        }

        public async Task<object> AsyncExecute(IDictionary<string, object> parameters)
        {
            var parametersArray = BuildInvokeParms(parameters);
            var instanceForCall = BuildInvokeInstance();
            var invokeRes = await Task.Run(() => MethodInfo.Invoke(instanceForCall, parametersArray));
            return invokeRes;
        }

        private object[] BuildInvokeParms(IDictionary<string, object> parameters)
        {
            var parameterDecs = GetParameterDescriptor();
            var parameterInfos = parameterDecs.Select(d => d.ParameterInfo);
            var rawParameterValues = from parameterInfo in parameterInfos
                                     select ExtractParameterFromDictionary(parameterInfo, parameters, MethodInfo);
            var parametersArray = rawParameterValues.ToArray();
            return parametersArray;
        }

        private object BuildInvokeInstance()
        {
            var instanceForCall = MethodInfo.IsStatic ? null : ServiceDescriptor.ServiceInstance;
            return instanceForCall;
        }

        internal static object ExtractParameterFromDictionary(ParameterInfo parameterInfo, IDictionary<string, object> parameters, MethodInfo methodInfo)
        {
            object value;
            if (!parameters.TryGetValue(parameterInfo.Name, out value))
            {
                // the key should always be present, even if the parameter value is null
                string message = String.Format(CultureInfo.CurrentCulture, "The parameters dictionary does not contain an entry for parameter '{0}' of type '{1}' for method '{2}' in '{3}'. The dictionary must contain an entry for each parameter, including parameters that have null values.",
                                               parameterInfo.Name, parameterInfo.ParameterType, methodInfo, methodInfo.DeclaringType);
                throw new ArgumentException(message, "parameters");
            }
            if (value == null && !TypeHelpers.TypeAllowsNullValue(parameterInfo.ParameterType))
            {
                // tried to pass a null value for a non-nullable parameter type
                string message = String.Format(CultureInfo.CurrentCulture, "The parameters dictionary contains a null entry for parameter '{0}' of non-nullable type '{1}' for method '{2}' in '{3}'. An optional parameter must be a reference type, a nullable type, or be declared as an optional parameter.",
                                               parameterInfo.Name, parameterInfo.ParameterType, methodInfo, methodInfo.DeclaringType);
                throw new ArgumentException(message, "parameters");
            }
            if (value != null && !parameterInfo.ParameterType.IsInstanceOfType(value))
            {
                // value was supplied but is not of the proper type
                string message = String.Format(CultureInfo.CurrentCulture, "The parameters dictionary contains an invalid entry for parameter '{0}' for method '{1}' in '{2}'. The dictionary contains a value of type '{3}', but the parameter requires a value of type '{4}'.",
                                               parameterInfo.Name, methodInfo, methodInfo.DeclaringType, value.GetType(), parameterInfo.ParameterType);
                throw new ArgumentException(message, "parameters");
            }
            return value;
        }

    }
}