using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Zx.Web.ApiAdmin.Controllers
{
    public class AdminController : Controller
    {
        #region service
        private IApiContainerService ApiContainerService
        {
            get { return ServiceFactory.GetSrv<IApiContainerService>(); }
        }
        #endregion

        #region property
        private string TmpLibPath
        {
            get { return Server.MapPath("~/TmpLib"); }
        }

        private string BackupPath
        {
            get { return Server.MapPath("~/Backup"); }
        }
        #endregion

        #region view

        /// <summary>
        /// 预览
        /// </summary>
        /// <returns></returns>
        public ViewResult Overview(int? siteId)
        {
            var sites = ApiContainerService.GetApiContainerSites();
            var model =
                sites.Select(s => new Tuple<int, string, int>(s.ID, s.ApiDomain, siteId.GetValueOrDefault())).ToList();
            ViewBag.SiteId = siteId.GetValueOrDefault();
            return View(model);
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <returns></returns>
        public ViewResult UploadDll(int? siteId)
        {
            var sites = ApiContainerService.GetApiContainerSites();
            var model =
                sites.Select(
                    s =>
                        new Tuple<int, string, string, string>(s.ID, s.ApiDomain, GetApiLibPathStr(s),
                            s.Description ?? string.Empty)).ToList();
            ViewBag.SiteId = siteId.GetValueOrDefault();
            return View(model);
        }

        private string GetApiLibPathStr(ApiContainerSiteConfig site)
        {
            if (site == null) return string.Empty;
            var strMachines = site.Machine ?? string.Empty;
            var paths = strMachines.Split(',').Select(m => string.Format(@"\\{0}\{1}\", m, site.LibFolder ?? string.Empty));
            return string.Join(",", paths);
        }

        /// <summary>
        /// 回滚
        /// </summary>
        /// <returns></returns>
        public ViewResult Rollback(int? siteId)
        {
            var sites = ApiContainerService.GetApiContainerSites();
            var model = sites.Select(s => new Tuple<int, string, int>(s.ID, s.ApiDomain, siteId.GetValueOrDefault())).ToList();
            return View(model);
        }

        #endregion

        #region ajax

        #region 预览
        /// <summary>
        /// 获取站点当前mapping
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public JsonResult MappingInfos(int siteId)
        {
            var record = ApiContainerService.GetApiContainerLastUploadRecord(siteId);
            var mappings = ApiContainerService.GetApiContainerRouteMappingBySite(siteId);
            var mRes = mappings.Select(m => new
            {
                m.Verb,
                m.Path,
                Assembly = m.ServiceAssembly,
                Service = m.ServiceName,
                Method = m.MethodName,
                AsyncInvoke = m.IsAsyncInvoke.GetValueOrDefault().ToString()
            });
            var res = new
            {
                LastAddTime = record == null ? string.Empty : record.AddTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Remark = record == null ? string.Empty : record.Remark ?? string.Empty,
                Mappings = mRes
            };
            return Json(new { Res = res }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 上传
        /// <summary>
        /// 上传文件到临时目录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Upload()
        {
            var results = new List<UploadFileResult>();
            foreach (string file in Request.Files)
            {
                var hpf = Request.Files[file] as HttpPostedFileBase;
                if (hpf == null || hpf.ContentLength == 0) continue;
                var fileName = hpf.FileName;
                if (!FileHelper.CreateFolderIfNeeded(TmpLibPath)) continue;
                hpf.SaveAs(Path.Combine(TmpLibPath, fileName));
                results.Add(new UploadFileResult
                {
                    FilePath = Url.Content(String.Format("~/TmpLib/{0}", fileName)),
                    FileName = fileName,
                    IsValid = true,
                    Length = hpf.ContentLength,
                    Message = "上传成功",
                    Type = hpf.ContentType
                });
            }
            return Json(new
            {
                name = results[0].FileName,
                type = results[0].Type,
                size = string.Format("{0} bytes", results[0].Length),
                path = results[0].FilePath,
                msg = results[0].Message
            });
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteFile(string name)
        {
            System.IO.File.Delete(Path.Combine(TmpLibPath, name));
            return Json(new { msg = true });
        }

        /// <summary>
        /// 清空上传文件所需的临时目录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CleanTmpLib()
        {
            var res = FileHelper.CleanDirectory(TmpLibPath);
            return Json(new { msg = res });
        }

        /// <summary>
        /// 获取上传文件中的类（入口dll）
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JsonResult DllClass(string name)
        {
            var assemblyFile = TmpLibPath + "/" + name;
            var cls = ApiContainerService.GetAssembliesClassNames(assemblyFile);
            var res = cls.Select(c => new { Name = c });
            return Json(new { Res = res }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取上次更新中已出现过的类
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public JsonResult GetAppeardClass(int siteId)
        {
            var liRm = ApiContainerService.GetApiContainerRouteMappingBySite(siteId) ?? new List<ApiContainerRouteMapping>();
            var bms = liRm.Select(m => m.ServiceName).ToList();
            return Json(new { Res = bms }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取上传dll中类的方法（加入对比）
        /// </summary>
        /// <returns></returns>
        public JsonResult ClassMethod(ClassMethodModel model)
        {
            var assemblyFile = TmpLibPath + "/" + model.Dname;
            model.MethodsInDll = ApiContainerService.GetClassMethodNames(assemblyFile, model.Cname);
            model.MethodsInMapping =
                (ApiContainerService.GetApiContainerRouteMappingBySite(model.SiteId) ??
                 new List<ApiContainerRouteMapping>()).Where(m => m.ServiceName == model.Cname).ToList();
            return Json(new { Res = model.GetResWithCompare() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 构建mapping显示数据（加入默认值）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BuildMappingDataWithDefaultSetting(ClientMappingModel model)
        {
            model.AppearedMappings = ApiContainerService.GetApiContainerRouteMappingBySite(model.SiteId) ??
                                     new List<ApiContainerRouteMapping>();
            return Json(new { Res = model.GetResWithDefaultSetting() });
        }

        /// <summary>
        /// 开始同步容器
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="remark"></param>
        /// <param name="mappings"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Sync(int siteId, string remark, List<ClientMappingItemModel> mappings)
        {
            if (!ApiContainerService.SyncLib(TmpLibPath, siteId))
                return Json(new { res = -1 });
            var rm = mappings.Select(m => new ApiContainerRouteMapping
            {
                Verb = m.Verb,
                Path = m.Path,
                ServiceAssembly = m.ServiceAssembly,
                ServiceName = m.ServiceName,
                MethodName = m.MethodName,
                SiteId = siteId,
                IsAsyncInvoke = m.IsAsync
            }).ToList();
            var backUpFolder = ApiContainerService.GetBackupFolderName(siteId);
            if (!ApiContainerService.SyncDb(siteId, remark, backUpFolder, rm))
                return Json(new { res = -2 });
            var backupFullPath = BackupPath + "\\" + backUpFolder;
            if (!ApiContainerService.BackUp(siteId, TmpLibPath, backupFullPath))
                return Json(new { res = -3 });
            if (!ApiContainerService.RecycleAppPool(siteId))
                return Json(new { res = -4 });
            return Json(new { res = 0 });
        }

        #endregion

        #region 回滚
        /// <summary>
        /// 获取站点上传记录
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public JsonResult SiteUploadRecords(int siteId)
        {
            var records = ApiContainerService.GetApiContainerUploadRecord(siteId);
            var res = new List<object>();
            var num = 0;
            records.OrderByDescending(r => r.AddTime).ForEach(r =>
            {
                var item = new
                {
                    Id = r.ID,
                    AddTime = r.AddTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    r.Remark,
                    RowNum = num
                };
                num++;
                res.Add(item);
            });
            return Json(new { Res = res }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 回滚
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="rid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Rollback(int siteId, int rid)
        {
            var backRecord = ApiContainerService.GetApiContainerUploadRecordById(rid);
            var rollBackFromPath = BackupPath + "\\" + backRecord.BackupFolder;
            if (!ApiContainerService.SyncLib(rollBackFromPath, siteId))
                return Json(new { res = -1 });
            if (!ApiContainerService.RollbackApiContainerRouteMapping(siteId, rid))
                return Json(new { res = -2 });
            if (!ApiContainerService.RecycleAppPool(siteId))
                return Json(new { res = -3 });
            return Json(new { res = 0 });
        }
        #endregion

        #endregion

    }
}
