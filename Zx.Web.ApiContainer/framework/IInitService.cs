using System.Collections.Generic;
using Zx.Web.ApiContainer.model;

namespace Zx.Web.ApiContainer.framework
{
    public interface IInitService
    {
        List<RouteMapping> GetMappings();
    }
}
