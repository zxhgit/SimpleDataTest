using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zx.ApiAdmin.Utilities
{
    public static class FileHelper
    {
        public static bool CleanDirectory(string path)
        {
            if (!Directory.Exists(path)) return true;
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles();
            try
            {
                foreach (var item in files)
                {
                    File.Delete(item.FullName);
                }
                if (dir.GetDirectories().Length != 0)
                {
                    foreach (var item in dir.GetDirectories())
                    {
                        if (!item.ToString().Contains("$") && (!item.ToString().Contains("Boot")))
                            CleanDirectory(dir + "\\" + item);
                    }
                }
                Directory.Delete(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 检查是否要创建上传文件夹，如果没有就创建
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static bool CreateFolderIfNeeded(string path)
        {
            bool result = true;
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    //TODO：处理异常
                    result = false;
                }
            }
            return result;
        }

        public static void DirectoryCopy(string sourceDirectory, string targetDirectory, bool isRecursive = false)
        {
            if (!Directory.Exists(sourceDirectory) || !Directory.Exists(targetDirectory))
            {
                return;
            }
            var sourceInfo = new DirectoryInfo(sourceDirectory);
            var fileInfo = sourceInfo.GetFiles();
            foreach (var fiTemp in fileInfo)
            {
                File.Copy(sourceDirectory + "\\" + fiTemp.Name, targetDirectory + "\\" + fiTemp.Name, true);
            }
            if (!isRecursive) return;
            var diInfo = sourceInfo.GetDirectories();
            foreach (var diTemp in diInfo)
            {
                var sourcePath = diTemp.FullName;
                var targetPath = diTemp.FullName.Replace(sourceDirectory, targetDirectory);
                Directory.CreateDirectory(targetPath);
                DirectoryCopy(sourcePath, targetPath);
            }
        }
    }
}
