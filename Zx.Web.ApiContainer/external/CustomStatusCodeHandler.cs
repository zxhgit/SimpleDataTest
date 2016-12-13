using Nancy;
using Nancy.ErrorHandling;
using Nancy.Responses.Negotiation;
using Zx.Web.ApiContainer.ErrorHandler;
using Zx.Web.ApiContainer.model;
using Zx.Web.ApiContainer.model.Enum;


namespace Zx.Web.ApiContainer.external
{
    /// <summary>
    /// 500code默认返回处理,未经过ErrorPipeline处理
    /// </summary>
    /// <remarks>未经过ErrorPipeline处理</remarks>
    /// <remarks>处理ErrorHook时再出异常，则无法经过ErrorPipeline处理</remarks>
    public class StatusCodeHandler500 : IStatusCodeHandler
    {
        private IResponseNegotiator responseNegotiator;

        public StatusCodeHandler500(IResponseNegotiator responseNegotiator)
        {
            this.responseNegotiator = responseNegotiator;
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.InternalServerError;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            context.NegotiationContext = new NegotiationContext();
            var model = new ServiceError
            {
                Code = ServiceErrorCodeEnum.InternalServerError,
                Details = "There was an internal server error during processing the request."
            };
            Negotiator negotiator = new Negotiator(context)
                .WithStatusCode(HttpStatusCode.InternalServerError)
                .WithModel(model);

            context.Response = responseNegotiator.NegotiateResponse(negotiator, context);
        }
    }
    /// <summary>
    /// 404code默认返回处理,
    /// </summary>
    /// <remarks>未经过ErrorPipeline处理</remarks>
    /// <remarks>route匹配失败不会抛出异常</remarks>
    public class StatusCodeHandler404 : IStatusCodeHandler
    {
        private IResponseNegotiator responseNegotiator;

        public StatusCodeHandler404(IResponseNegotiator responseNegotiator)
        {
            this.responseNegotiator = responseNegotiator;
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.NotFound;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            context.NegotiationContext = new NegotiationContext();
            var model = new ServiceError
            {
                Code = ServiceErrorCodeEnum.PathMissing,
                Details = "path missing."
            };
            Negotiator negotiator = new Negotiator(context)
                .WithStatusCode(HttpStatusCode.NotFound)
                .WithModel(model);

            context.Response = responseNegotiator.NegotiateResponse(negotiator, context);
        }
    }

    /// <summary>
    /// 404code默认返回处理,
    /// </summary>
    /// <remarks>未经过ErrorPipeline处理</remarks>
    /// <remarks>route匹配失败不会抛出异常</remarks>
    public class StatusCodeHandler405 : IStatusCodeHandler
    {
        private IResponseNegotiator responseNegotiator;

        public StatusCodeHandler405(IResponseNegotiator responseNegotiator)
        {
            this.responseNegotiator = responseNegotiator;
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.MethodNotAllowed;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            context.NegotiationContext = new NegotiationContext();
            var model = new ServiceError
            {
                Code = ServiceErrorCodeEnum.MethodMissing,
                Details = "method missing."
            };
            Negotiator negotiator = new Negotiator(context)
                .WithStatusCode(HttpStatusCode.MethodNotAllowed)
                .WithModel(model);

            context.Response = responseNegotiator.NegotiateResponse(negotiator, context);
        }
    }
}