namespace Zx.ApiAdmin.Services.DataAdapters
{
    public interface IApiDataAdapter : IBasicDataAdapter
    {
        /// <summary>
        /// 删除指定api站点的RouteMapping
        /// </summary>
        /// <param name="siteId"></param>
        void DeleteApiContainerRouteMapping(int siteId);
    }
}