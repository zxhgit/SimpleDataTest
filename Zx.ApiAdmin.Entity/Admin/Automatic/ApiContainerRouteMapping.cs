using System;
using Zx.ApiAdmin.Entity.Attributes;

namespace Zx.ApiAdmin.Entity.Admin
{
    [Serializable]
    public class ApiContainerRouteMapping
    {
        /// <summary>
        /// id
        /// </summary>
        [PrimaryKeyAttribute]
        [IdentityAttribute]
        public int ID { get; set; }
        /// <summary>
        /// verb:get,post,put,delete
        /// </summary>
        public string Verb { get; set; }
        /// <summary>
        /// api的url路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 程序集文件名
        /// </summary>
        public string ServiceAssembly { get; set; }
        /// <summary>
        /// 服务类名（命名空间+类名）
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 方法名称
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 是否异步调用
        /// </summary>
        public bool? IsAsyncInvoke { get; set; }
        /// <summary>
        /// siteid
        /// </summary>
        public int SiteId { get; set; }
    }
}
