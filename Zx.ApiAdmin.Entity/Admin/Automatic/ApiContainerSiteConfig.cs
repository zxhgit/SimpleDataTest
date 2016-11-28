using System;
using Zx.ApiAdmin.Entity.Attributes;

namespace Zx.ApiAdmin.Entity.Admin
{
    [Serializable]
    public class ApiContainerSiteConfig
    {
        /// <summary>
        /// id
        /// </summary>
        [PrimaryKeyAttribute]
        [IdentityAttribute]
        public int ID { get; set; }
        /// <summary>
        /// 域名
        /// </summary>
        public string ApiDomain { get; set; }
        /// <summary>
        /// 容器dll目录
        /// </summary>
        public string LibFolder { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 所在机器
        /// </summary>
        public string Machine { get; set; }
    }
}
