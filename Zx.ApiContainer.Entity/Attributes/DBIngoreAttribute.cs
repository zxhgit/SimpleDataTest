using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zx.ApiContainer.Entity.Attributes
{
    /// <summary>
    /// 自定义的数据库忽略属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DBIngoreAttribute : Attribute
    {
    }
}
