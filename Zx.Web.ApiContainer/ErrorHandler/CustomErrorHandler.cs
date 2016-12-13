using Nancy;
using Nancy.Bootstrapper;
using Nancy.Responses.Negotiation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zx.Web.ApiContainer.external;
using Zx.Web.ApiContainer.framework;
using Zx.Web.ApiContainer.model;
using Zx.Web.ApiContainer.model.Enum;

namespace Zx.Web.ApiContainer.ErrorHandler
{
    public static class CustomErrorHandler
    {
        public static void Enable(IPipelines pipelines, IResponseNegotiator responseNegotiator)
        {
            if (pipelines == null)
            {
                throw new ArgumentNullException("pipelines");
            }

            if (responseNegotiator == null)
            {
                throw new ArgumentNullException("responseNegotiator");
            }

            pipelines.OnError += (context, exception) => HandleException(context, exception, responseNegotiator);
        }

        private static void LogException(NancyContext context, Exception exception)
        {
            if (exception is BadRequestErrorException) return;
            var internalEx = exception as InternalServerErrorException;
            if (internalEx != null)
                exception = internalEx.ExceptionForLog;
            //ExceptionLogService.LogException(exception);
            var logSrv = EntryModule.IocResolve<IExceptionLogService>();
            logSrv.LogError(exception);
        }

        private static Response HandleException(NancyContext context, Exception exception, IResponseNegotiator responseNegotiator)
        {
            exception = ExceptionConvert(exception);
            LogException(context, exception);
            return CreateNegotiatedResponse(context, responseNegotiator, exception);
        }

        private static Response CreateNegotiatedResponse(NancyContext context, IResponseNegotiator responseNegotiator, Exception exception)
        {
            var defaultError = new HttpServiceError
            {
                HttpStatusCode = HttpStatusCode.InternalServerError,
                ServiceError = new ServiceError
                {
                    Code = ServiceErrorCodeEnum.InternalServerError,
                    Details = "There was an internal server error during processing the request."
                }
            };
            var httpServiceError = HttpServiceErrorUtilities.ExtractFromException(exception, defaultError);
            Negotiator negotiator = new Negotiator(context)
                .WithStatusCode(httpServiceError.HttpStatusCode)
                .WithModel(httpServiceError.ServiceError);
            return responseNegotiator.NegotiateResponse(negotiator, context);
        }

        private static Exception ExceptionConvert(Exception exception)
        {
            if (exception is ParameterNullException || exception is ParameterTypeException ||
                exception is ParameterFormatException)
                return new BadRequestErrorException((ServiceException)exception);


            if (exception is PlatformErrorException || exception is InternalErrorException)
                //return new InternalServerErrorException(exception);
                return new Exception(exception.Message, exception.InnerException);
            return exception;
        }
    }
}