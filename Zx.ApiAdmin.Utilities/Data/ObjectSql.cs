using System.Data;

namespace Zx.ApiAdmin.Utilities.Data
{
    public class ObjectSql
    {
        /// <summary>
        /// 数据库链接在配置中的名称
        /// </summary>
        public string DBKey
        {
            get;
            set;
        }

        /// <summary>
        /// SQL标识
        /// </summary>
        public string SQLName
        {
            get;
            set;
        }

        /// <summary>
        /// SQL文本
        /// </summary>
        public string SQLText
        {
            get;
            set;
        }

        /// <summary>
        /// SQL分页OrderBy部分
        /// </summary>
        public string OrderBy
        {
            get;
            set;
        }


        /// <summary>
        /// SQL的类型
        /// </summary>
        public CommandType CommandType
        {
            get;
            set;
        }

        /// <summary>
        /// 等待命令执行的时间（以秒为单位）。默认值为 30 秒。 值 0 指示无限制，应尽量避免值 0，否则会无限期地等待执行命令。
        /// </summary>
        public int CommandTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public ObjectSql()
        {
            this.CommandTimeout = 30;
            this.CommandType = CommandType.Text;
        }
    }
}
