using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zx.ApiAdmin.Entity.Admin;

namespace Zx.ApiAdmin.Wrappers
{
    public interface IApiContainerService : IBasicService
    {
        /// <summary>
        /// 获取api现有站点
        /// </summary>
        /// <returns></returns>
        List<ApiContainerSiteConfig> GetApiContainerSites();

        /// <summary>
        /// 获取最新更新记录
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        ApiContainerUploadRecord GetApiContainerLastUploadRecord(int siteId);

        /// <summary>
        /// 根据siteId获取api站点的路由映射
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        List<ApiContainerRouteMapping> GetApiContainerRouteMappingBySite(int siteId);

        /// <summary>
        /// 获取指定站点更新记录
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        List<ApiContainerUploadRecord> GetApiContainerUploadRecord(int siteId);

        /// <summary>
        /// 获取指定更新记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ApiContainerUploadRecord GetApiContainerUploadRecordById(int id);

        /// <summary>
        /// 同步lib目录
        /// </summary>
        /// <param name="sourceLibPath"></param>
        /// <param name="siteId"></param>
        /// <returns></returns>
        bool SyncLib(string sourceLibPath, int siteId);

        /// <summary>
        /// 同步db
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="remark"></param>
        /// <param name="backUpFolderName"></param>
        /// <param name="mappings"></param>
        /// <returns></returns>
        bool SyncDb(int siteId, string remark, string backUpFolderName, List<ApiContainerRouteMapping> mappings);

        /// <summary>
        /// 备份
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="tmpLibPath"></param>
        /// <param name="backupFullPath"></param>
        /// <returns></returns>
        bool BackUp(int siteId, string tmpLibPath, string backupFullPath);

        /// <summary>
        /// 获取备份文件夹名
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        string GetBackupFolderName(int siteId);

        /// <summary>
        /// 回滚Mapping
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="recordId"></param>
        /// <returns></returns>
        bool RollbackApiContainerRouteMapping(int siteId, int recordId);

        /// <summary>
        /// 获取程序集中的类名
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <returns></returns>
        List<string> GetAssembliesClassNames(string assemblyFile);

        /// <summary>
        /// 获取指定程序集类中的方法
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <param name="classFullName"></param>
        /// <returns></returns>
        List<ApiContainerMethodInfo> GetClassMethodNames(string assemblyFile, string classFullName);

        /// <summary>
        /// ApiContainerUpLoad更新数据
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="remark"></param>
        /// <param name="folderName"></param>
        /// <param name="mappings"></param>
        /// <returns></returns>
        bool ApiContainerUpLoad(int siteId, string remark, string folderName, List<ApiContainerRouteMapping> mappings);

        /// <summary>
        /// 回收应用程序池
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        bool RecycleAppPool(int siteId);
    }
}
