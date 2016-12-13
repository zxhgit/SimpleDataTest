using System.Linq;
using Nancy;
using System;
using System.Collections.Generic;
using System.Globalization;
using Zx.Web.ApiContainer.ErrorHandler;
using Zx.Web.ApiContainer.model.Enum;
using System.Threading.Tasks;

namespace Zx.Web.ApiContainer.core
{
    public class MethodInvoker
    {
        private readonly IValueProvider _valueProvider;

        public DefaultModelBinder ModelBinder
        {
            get { return new DefaultModelBinder(); }
        }

        public MethodInvoker(NancyContext context)
        {
            var providerFactories = new List<ValueProviderFactory>
            {
                new FormValueProviderFactory(),
                new JsonValueProviderFactory(),
                new RouteValueProviderFactory(),
                new QueryValueProviderFactory()
            };
            var factoryCollection = new ValueProviderFactoryCollection(providerFactories);
            _valueProvider = factoryCollection.GetValueProvider(context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodDec"></param>
        /// <returns></returns>
        public object Invoke(MethodDescriptor methodDec)
        {
            IDictionary<string, object> parameterValues;
            try
            {
                parameterValues = GetParameterValues(methodDec);
            }
            catch (ParameterTypeException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new PlatformErrorException(e);
            }
            try
            {
                var res = methodDec.Execute(parameterValues);
                return res;
            }
            catch (Exception e)
            {
                throw new InternalErrorException(e);
            }
        }

        public async Task<object> AsyncInvoke(MethodDescriptor methodDec)
        {
            IDictionary<string, object> parameterValues;
            try
            {
                parameterValues = GetParameterValues(methodDec);
            }
            catch (ParameterTypeException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new PlatformErrorException(e);
            }
            try
            {
                var res = await methodDec.AsyncExecute(parameterValues);
                return res;
            }
            catch (Exception e)
            {
                throw new InternalErrorException(e);
            }
        }

        /// <summary>
        /// 获取参数的值
        /// </summary>
        /// <returns></returns>
        private object GetParameterValue(ParameterDescriptor parameterDescriptor)
        {
            var bindResult = ModelBinder.BindModel(new ModelBindingContext
            {
                ModelType = parameterDescriptor.ParameterType,
                ModelName = parameterDescriptor.ParameterName,
                ValueProvider = _valueProvider
            });
            var res = bindResult ?? parameterDescriptor.DefaultValue;
            if (res == DBNull.Value)
                throw new ParameterNullException(parameterDescriptor.ParameterName);
            return res;
        }

        private IDictionary<string, object> GetParameterValues(MethodDescriptor methodDescriptor)
        {
            var parametersDict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var parameterDescriptors = methodDescriptor.GetParameterDescriptor();
            foreach (var parameterDescriptor in parameterDescriptors)
            {
                parametersDict[parameterDescriptor.ParameterName] = GetParameterValue(parameterDescriptor);
            }
            return parametersDict;
        }
    }
}