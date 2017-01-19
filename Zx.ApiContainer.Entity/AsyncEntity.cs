using System;
using System.Collections.Generic;
using Zx.ApiContainer.Entity.Attributes;

namespace Zx.ApiContainer.Entity
{
    /// <summary>
    /// 异步处理消息实体
    /// </summary>
    [Serializable]
    public class AsyncEntity
    {
        /// <summary>
        /// 子项
        /// </summary>
        [DBIngoreAttribute]
        public virtual List<AsyncEntity> Children
        {
            get;
            set;
        }

        /// <summary>
        /// 父ID
        /// </summary>
        [DBIngoreAttribute]
        public virtual long? ParentID
        {
            get;
            set;
        }

        /// <summary>
        /// 当前主键
        /// </summary>
        [DBIngoreAttribute]
        public virtual long PrimaryKey
        {
            get;
            set;
        }
    }
}
