using System;
using System.Collections.Generic;

namespace Zx.ApiAdmin.Entity.Configration
{
    public class InstanceStoredEntity
    {
        /// <summary>
        /// 当前实体的属性组
        /// </summary>
        public Dictionary<string, PropertyStoredEntity> Properties
        {
            get;
            set;
        }

        /// <summary>
        /// 通过属性名称获取一个属性
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        public PropertyStoredEntity GetProperty(string propertyName)
        {
            if (this.Properties != null && this.Properties.Count > 0)
            {
                if (this.Properties.ContainsKey(propertyName.ToLower()))
                {
                    return this.Properties[propertyName.ToLower()];
                }

            }

            return null;
        }

        /// <summary>
        /// 实例的类型（或直接设定为泛型实体的类型）
        /// </summary>
        public Type InstanceType
        {
            get;
            set;
        }

        /// <summary>
        /// 实体对应的数据库中的表名称（或者是自定义的类型名称）
        /// </summary>
        public string TableName
        {
            get;
            set;
        }

        /// <summary>
        /// 主键属性
        /// </summary>
        public PropertyStoredEntity PrimaryKeyProperty
        {
            get;
            set;
        }

        /// <summary>
        /// 标识属性
        /// </summary>
        public PropertyStoredEntity IdentityProperty
        {
            get;
            set;
        }


    }
}
