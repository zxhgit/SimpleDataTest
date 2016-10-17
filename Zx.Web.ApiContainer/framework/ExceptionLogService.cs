using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using Nancy.Diagnostics;
using Zx.ApiContainer.Wrappers;
using Zx.Web.ApiContainer.external;

namespace Zx.Web.ApiContainer.framework
{
    public class ExceptionLogService : IExceptionLogService
    {
        //private static readonly string LibPath = HttpContext.Current.Server.MapPath("/") + "/corelib/";
        private const string ServiceAssembley = "ZhaoPin.HighEnd.Wrappers.dll";
        private const string ServiceName = "ZhaoPin.HighEnd.Wrappers.LogHelper";
        private const string MethodName = "LogException";

        private static MethodInfo _methodInfoCache;

        private static string LibPath
        {
            get
            {
                return HttpContext.Current.Server.MapPath("/") + "/corelib/";
            }
        }

        [Obsolete]
        public static void LogException(Exception ex)
        {
            var callback = new WaitCallback(DoLog);
            ThreadPool.QueueUserWorkItem(callback, ex);
        }

        private static void DoLog(object parm)
        {
            var obj = parm as Tuple<string, Exception>;
            if (obj == null) return;
            var path = obj.Item1;
            var ex = obj.Item2;
            if (path == null || ex == null) return;
            var methodInfo = GetMethodInfo(path);
            if (methodInfo == null) return;
            methodInfo.Invoke(null, new object[] { ex });
        }

        private static MethodInfo GetMethodInfo(string libPath)
        {
            var existingCache = Interlocked.CompareExchange(ref _methodInfoCache, null, null);
            if (existingCache != null) return existingCache;
            var assemblies = Assembly.LoadFrom(libPath + ServiceAssembley);
            var serviceType = assemblies.GetType(ServiceName);
            var methodInfo = serviceType.GetMethod(MethodName, new[] { typeof(Exception) });
            var updatedCache = Interlocked.CompareExchange(ref _methodInfoCache, methodInfo, null);
            return updatedCache ?? methodInfo;
        }

        public void LogError1(Exception ex)
        {
            var callback = new WaitCallback(DoLog);
            //实现异步写日志，注意callback所在的线程不是处理当前请求的线程。
            //所以无法获取如HttpContext.Current中的数据
            //需要在当前线程中先确定LibPath，避免在另一个线程中获取LibPath
            var parm = new Tuple<string, Exception>(LibPath, ex);
            ThreadPool.QueueUserWorkItem(callback, parm);
        }

        public void LogError(Exception ex)
        {
            LogHelper.LogException(ex);
        }
    }
}