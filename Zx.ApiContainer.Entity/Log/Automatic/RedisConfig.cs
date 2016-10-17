using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zx.ApiContainer.Entity.Attributes;

namespace Zx.ApiContainer.Entity.Log.Automatic
{
    [Serializable]
    public class RedisConfig
    {
        [PrimaryKeyAttribute]
        [IdentityAttribute]
        public int Id { get; set; }

        /// <summary>
        /// Redis地址
        /// </summary>
        public string RedisHost { get; set; }

        /// <summary>
        /// Redis端口号
        /// </summary>
        public string RedisPort { get; set; }
    }
}
