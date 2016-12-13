using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zx.ApiContainer.Entity.ApiEntity.Automatic;
using Zx.ApiContainer.Services.DataAdapters;
using Zx.ApiContainer.Wrappers;

namespace Zx.ApiContainer.Services
{
    public class ApiService : IApiService
    {
        private IApiDataAdapter GetDataAdapter()
        {
            return DataAdapterFactory.GetApiDataAdapter();
        }

        public List<ApiContainerRouteMapping> GetMappings(int siteId)
        {
            var adapter = GetDataAdapter();
            var mapping = adapter.Query<ApiContainerRouteMapping>("SiteId=@siteId",
                new Dictionary<string, object> { { "@siteId", siteId } });
            return mapping ?? new List<ApiContainerRouteMapping>();
        }
    }
}
