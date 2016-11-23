using System;
using Nancy;
using Zx.Web.ApiContainer.model;
using Zx.Web.ApiContainer.model.Enum;

namespace Zx.Web.ApiContainer.ErrorHandler
{
    public interface IHasHttpServiceError
    {
        HttpServiceError HttpServiceError { get; }
    }

    public abstract class ServiceException : Exception
    {
        protected ServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ServiceException(string message)
            : base(message)
        {
        }

        public virtual ServiceErrorCodeEnum ServiceErrorCode
        {
            get { return ServiceErrorCodeEnum.InternalServerError; }
        }

        public virtual Exception RealException
        {
            get { return InnerException; }
        }

        public ServiceError GetServiceError()
        {
            return new ServiceError
            {
                Code = ServiceErrorCode,
                Details = Message
            };
        }
    }

    public abstract class ParameterException : ServiceException
    {
        private const string MessageFormat = "{0}参数名:{1}";

        protected ParameterException(string paramName, string message)
            : base(string.Format(MessageFormat, message, paramName))
        {
        }

        public override Exception RealException
        {
            get { return this; }
        }
    }

    #region 异常主分类

    //500,404...

    public class BadRequestErrorException : Exception, IHasHttpServiceError
    {
        public readonly HttpStatusCode HttpStatusCode = HttpStatusCode.BadRequest;

        public BadRequestErrorException(ServiceException innerException)
            : base("", innerException)
        {
        }

        public HttpServiceError HttpServiceError
        {
            get
            {
                return HttpServiceErrorUtilities.GetHttpServiceError(HttpStatusCode,
                    ((ServiceException)InnerException).GetServiceError());
            }
        }
    }

    public class InternalServerErrorException : Exception, IHasHttpServiceError
    {
        public readonly HttpStatusCode HttpStatusCode = HttpStatusCode.InternalServerError;

        public InternalServerErrorException(ServiceException innerException)
            : base(innerException.Message, innerException)
        {
        }

        public Exception ExceptionForLog
        {
            get { return ((ServiceException)InnerException).RealException; }
        }

        public HttpServiceError HttpServiceError
        {
            get
            {
                return HttpServiceErrorUtilities.GetHttpServiceError(HttpStatusCode,
                    ((ServiceException)InnerException).GetServiceError());
            }
        }
    }

    #endregion

    #region 异常子分类

    #region 输入参数异常

    public class ParameterNullException : ParameterException
    {
        private const string ErrMessage = "无参数！";

        public override ServiceErrorCodeEnum ServiceErrorCode
        {
            get
            {
                return ServiceErrorCodeEnum.NullParameter;
            }
        }

        public ParameterNullException(string paramName, string message = ErrMessage)
            : base(paramName, message)
        {
        }
    }

    public class ParameterTypeException : ParameterException
    {
        private const string ErrMessage = "参数类型错误！";

        public override ServiceErrorCodeEnum ServiceErrorCode
        {
            get
            {
                return ServiceErrorCodeEnum.ParameterTypeErr;
            }
        }

        public ParameterTypeException(string paramName, string message = ErrMessage)
            : base(paramName, message)
        {
        }
    }

    public class ParameterFormatException : ServiceException
    {
        private const string ErrMessageFormat = "参数格式错误，详细信息：{0}";

        public override ServiceErrorCodeEnum ServiceErrorCode
        {
            get
            {
                return ServiceErrorCodeEnum.ParameterFormatErr;
            }
        }

        public ParameterFormatException(Exception innerException)
            : base(string.Format(ErrMessageFormat, innerException.Message), innerException)
        {
        }
    }

    #endregion

    #region 内部异常

    public class PlatformErrorException : ServiceException
    {
        private const string ErrMessageFormat = "平台异常,详细信息：{0}";

        public PlatformErrorException(Exception innerException)
            : base(string.Format(ErrMessageFormat, innerException.Message), innerException)
        {
        }
    }

    public class InternalErrorException : ServiceException
    {
        private const string ErrMessageFormat = "内部服务异常,详细信息：{0}";

        public InternalErrorException(Exception innerException)
            : base(string.Format(ErrMessageFormat, innerException.Message), innerException)
        {
        }
    }

    #endregion

    #endregion
}