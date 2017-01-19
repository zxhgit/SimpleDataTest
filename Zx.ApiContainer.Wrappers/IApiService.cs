using System.Collections.Generic;
using Zx.ApiContainer.Entity.ApiEntity.Automatic;

namespace Zx.ApiContainer.Wrappers
{
    public interface IApiService : IBasicService
    {
        List<ApiContainerRouteMapping> GetMappings(int siteId);
    }
}
