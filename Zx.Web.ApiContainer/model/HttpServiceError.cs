using Nancy;

namespace Zx.Web.ApiContainer.model
{
    public class HttpServiceError
    {
        public ServiceError ServiceError { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}