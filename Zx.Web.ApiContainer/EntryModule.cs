using System.ComponentModel;
using Nancy;
using Nancy.Responses.Negotiation;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Nancy.TinyIoc;
using Zx.Web.ApiContainer.core;
using Zx.Web.ApiContainer.ErrorHandler;
using Zx.Web.ApiContainer.external;
using Zx.Web.ApiContainer.framework;
using Zx.Web.ApiContainer.model;
using Zx.Web.ApiContainer.model.Enum;
using Nancy.Security;

namespace Zx.Web.ApiContainer
{
    public class EntryModule : NancyModule
    {
        internal static readonly string LibPath = HttpContext.Current.Server.MapPath("/") + "/apilib/";//lib文件目录可以由siteid获得——表ApiContainerSiteConfig
        private static readonly Dictionary<Tuple<string, string>, MethodDescriptor> MethodContainer =
            new Dictionary<Tuple<string, string>, MethodDescriptor>();

        private static readonly TinyIoCContainer Ioc = new TinyIoCContainer();

        static EntryModule()
        {
            try
            {
                ConfigureIoc();
                var initService = IocResolve<IInitService>();
                var mappings = initService.GetMappings();
                mappings.ForEach(
                    m =>
                        MethodContainer.Add(new Tuple<string, string>(m.Verb, m.Path),
                            MethodDescriptor.GetDescriptorInstance(m)));
            }
            catch (Exception e)
            {
                var logSrv = IocResolve<IExceptionLogService>();
                logSrv.LogError(new Exception("启动错误！", e));
            }
        }

        public EntryModule()
        {
            try
            {
                if (AuthService.IsAuth)
                    this.RequiresAuthentication();//开启Authen
                foreach (var m in MethodContainer)
                {
                    var verb = m.Key.Item1.ToLower();
                    var path = m.Key.Item2;
                    var methodDescriptor = m.Value;
                    //是否异步要看上传时的配置，取决于service方法是否是io密集型
                    switch (verb)
                    {
                        case "get":
                            SetGet(path, methodDescriptor);
                            break;
                        case "post":
                            SetPost(path, methodDescriptor);
                            break;
                        case "put":
                            break;
                        case "delete":
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                throw new PlatformErrorException(e);
            }
        }

        private static void ConfigureIoc()
        {
            //Ioc.Register<IInitService>(new InitService()).AsSingleton();//该写法不能AsSingleton
            Ioc.Register(typeof(IInitService), typeof(InitServiceByDB)).AsSingleton();
            Ioc.Register(typeof(IExceptionLogService), typeof(ExceptionLogService)).AsSingleton();
        }

        public static T IocResolve<T>() where T : class
        {
            return Ioc.Resolve<T>();
        }

        private void SetGet(string path, MethodDescriptor descriptor)
        {
            if (descriptor.IsAsyncInvoke)
                Get[path, true] = GetAsyncDelegate(descriptor);
            else
                Get[path] = GetDelegate(descriptor);
        }

        private void SetPost(string path, MethodDescriptor descriptor)
        {
            if (descriptor.IsAsyncInvoke)
                Post[path, true] = GetAsyncDelegate(descriptor);
            else
                Post[path] = GetDelegate(descriptor);
        }

        private Func<dynamic, dynamic> GetDelegate(MethodDescriptor descriptor)
        {
            Func<dynamic, dynamic> dgt = p =>
            {
                var invoker = new MethodInvoker(Context);
                var res = invoker.Invoke(descriptor);
                //var apiRes = buildResult(res);
                //var jsonRes = Response.AsJson(apiRes);
                var jsonRes = Response.AsJson(res);//appservice已经封装了返回结果
                return jsonRes;
            };
            return dgt;
        }

        private Func<dynamic, CancellationToken, Task<dynamic>> GetAsyncDelegate(MethodDescriptor descriptor)
        {
            Func<dynamic, CancellationToken, Task<dynamic>> dgt = async (p, ct) =>
            {
                var invoker = new MethodInvoker(Context);
                var res = await invoker.AsyncInvoke(descriptor);
                //var apiRes = buildResult(res);
                //var jsonRes = Response.AsJson(apiRes);
                var jsonRes = Response.AsJson(res);//appservice已经封装了返回结果
                return jsonRes;
            };
            return dgt;
        }

        private ApiResult buildResult(object result)
        {
            var apiResult = new ApiResult { Code = ServiceErrorCodeEnum.NoError, Data = result };
            return apiResult;
        }
    }




}