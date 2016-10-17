using Zx.Web.ApiContainer.model.Enum;

namespace Zx.Web.ApiContainer.model
{
    public class ServiceError
    {
        public ServiceErrorCodeEnum Code { get; set; }
        public string Details { get; set; }
    }
}