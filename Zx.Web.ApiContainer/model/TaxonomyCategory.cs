
namespace Zx.Web.ApiContainer.model
{
    public class TaxonomyCategory
    {
        /// <summary>
        /// 词典Key值
        /// </summary>
        public string CategoryKey
        {
            get;
            set;
        }
        /// <summary>
        /// 英文名称
        /// </summary>
        public string CategoryENName
        {
            get;
            set;
        }
        /// <summary>
        /// 中文名称
        /// </summary>
        public string CategoryCNName
        {
            get;
            set;
        }
        /// <summary>
        /// 数值类型（尽量用小类型tinyint smallint）
        /// </summary>
        public string ItemType
        {
            get;
            set;
        }
        /// <summary>
        /// Levels
        /// </summary>
        public byte? Levels
        {
            get;
            set;
        }
    }
}