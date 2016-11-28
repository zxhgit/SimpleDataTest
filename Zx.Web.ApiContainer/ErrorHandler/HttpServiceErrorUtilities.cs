using Nancy;
using System;
using Zx.Web.ApiContainer.model;
using Zx.Web.ApiContainer.model.Enum;

namespace Zx.Web.ApiContainer.ErrorHandler
{
    public static class HttpServiceErrorUtilities
    {
        public static HttpServiceError ExtractFromException(Exception exception, HttpServiceError defaultValue)
        {
            var result = defaultValue;
            if (exception == null) return result;
            var exceptionWithServiceError = exception as IHasHttpServiceError;
            if (exceptionWithServiceError != null)
                result = exceptionWithServiceError.HttpServiceError;
            return result;
        }

        public static HttpServiceError GetHttpServiceError(HttpStatusCode httpCode, ServiceErrorCodeEnum serviceCode,
            string details)
        {
            var error = new HttpServiceError
            {
                HttpStatusCode = httpCode,
                ServiceError = new ServiceError
                {
                    Code = serviceCode,
                    Details = details
                }
            };
            return error;
        }

        public static HttpServiceError GetHttpServiceError(HttpStatusCode httpCode, ServiceError serviceErr)
        {
            var error = new HttpServiceError
            {
                HttpStatusCode = httpCode,
                ServiceError = serviceErr
            };
            return error;
        }
    }
}