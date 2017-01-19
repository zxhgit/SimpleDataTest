using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Zx.ApiContainer.Entity.Configration
{
    /// <summary>
    /// 实体属性（属性数据类型、是否是主键、是否自增列）
    /// </summary>
    public class PropertyStoredEntity
    {
        /// <summary>
        /// 当前属性
        /// </summary>
        public PropertyInfo CurrentProperty
        {
            get;
            set;
        }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType
        {
            get;
            set;
        }

        /// <summary>
        /// 是自增列
        /// </summary>
        public bool IsIdentity
        {
            get;
            set;
        }

        /// <summary>
        /// 是主键
        /// </summary>
        public bool IsPrimaryKey
        {
            get;
            set;
        }
    }
}
