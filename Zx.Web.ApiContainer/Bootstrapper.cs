using Nancy;
using Nancy.Authentication.Token;
using Nancy.Bootstrapper;
using Nancy.Diagnostics;
using Nancy.Responses.Negotiation;
using Nancy.TinyIoc;
using System;
using Zx.Web.ApiContainer.ErrorHandler;
using Zx.Web.ApiContainer.external;
using Zx.Web.ApiContainer.framework;

namespace Zx.Web.ApiContainer
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get { return new DiagnosticsConfiguration { Password = @"111111" }; }
        }

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides(config =>
                {
                    config.StatusCodeHandlers = new[] { typeof(StatusCodeHandler404), typeof(StatusCodeHandler500), typeof(StatusCodeHandler405) };
                    config.ResponseProcessors = new[] { typeof(JsonProcessor), typeof(XmlProcessor) };
                });
            }
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            CustomErrorHandler.Enable(pipelines, container.Resolve<IResponseNegotiator>());
            if (AuthService.IsAuth)
                TokenAuthentication.Enable(pipelines,
                    new TokenAuthenticationConfiguration(container.Resolve<ITokenizer>()));
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.Register<ITokenizer>(
                new Tokenizer(
                    cfg => cfg.TokenExpiration(() => TimeSpan.FromMinutes(5))
                        .KeyExpiration(() => TimeSpan.FromMinutes(10))));
        }


    }
}