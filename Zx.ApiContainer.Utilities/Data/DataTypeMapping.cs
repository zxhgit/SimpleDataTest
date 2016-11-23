namespace Zx.ApiContainer.Utilities.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class DataTypeMapping
    {
        /// <summary>
        /// 通过系统Property类型映射SQL类型字符串
        /// </summary>
        /// <param name="programeTypeString">系统Property类型字符串</param>
        /// <returns></returns>
        public static string GetSqlTypeString(string programeTypeString)
        {
            string smallFormat = programeTypeString.ToLower();

            string sqlFormat = null;

            switch (smallFormat)
            {
                case "int32":
                    sqlFormat = "int";
                    break;
                case "int64":
                    sqlFormat = "bigint";
                    break;
                case "int16":
                    sqlFormat = "smallint";
                    break;
                case "byte":
                    sqlFormat = "tinyint";
                    break;
                case "boolen":
                    sqlFormat = "bit";
                    break;
            }

            return sqlFormat;
        }
    }
}
