using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zx.ApiAdmin.Entity.Attributes;

namespace Zx.ApiAdmin.Entity.Admin
{
    [Serializable]
    public class ApiContainerUploadRecord
    {
        /// <summary>
        /// id
        /// </summary>
        [PrimaryKeyAttribute]
        [IdentityAttribute]
        public int ID { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 注释
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 站点id
        /// </summary>
        public int SiteId { get; set; }
        /// <summary>
        /// 备份文件夹名
        /// </summary>
        public string BackupFolder { get; set; }
    }
}
