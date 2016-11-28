using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Configuration;

namespace Zx.ApiAdmin.Utilities.Data
{
    public static class DBAccessor
    {

        #region Constants

        /// <summary>
        /// 配置库DBkey
        /// </summary>
        //const string CONFIG_DBKEY = "ZhaoPin.HighEnd.ConfigDB";
        const string CONFIG_DBKEY = "Zx.ConfigDB";

        /// <summary>
        /// 默认的SQL文件目录 "SQLMap"
        /// </summary>
        const string SQLMAPPING_FILEPATH = "SQLMap";

        /// <summary>
        /// 默认的SQL文件格式 "*.sql.xml"
        /// </summary>
        const string SQLMAPPING_FILEPATTERN = "*.sql.xml";

        /// <summary>
        /// 初始化所有连接串的SQL语句
        /// </summary>
        const string INIT_CONNECTIONS_SQL = "Select DBKey,ConnectionString from DBConnections";

        /// <summary>
        /// 默认执行SQL超时时间
        /// </summary>
        const int DEFAULT_EXECUTE_SQL_TIMEOUT = 30;

        #endregion

        #region 属性

        /// <summary>
        /// 单点锁定
        /// </summary>
        static object _LockPad = new object();

        /// <summary>
        /// 存储Sql语句的词典（具体的查询语句级别）
        /// </summary>
        public static Dictionary<string, ObjectSql> SqlMap
        {
            get;
            private set;
        }

        /// <summary>
        /// 存储数据库配置节的词典（数据库级别）
        /// </summary>
        public static Dictionary<string, DataBase> DBMap
        {
            get;
            private set;
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 
        /// </summary>
        static DBAccessor()
        {
            Init();
        }

        /// <summary>
        /// 
        /// </summary>
        static void Init()
        {
            if (DBMap == null)
            {
                lock (_LockPad)
                {
                    if (DBMap == null)
                    {
                        //加载数据库连接串
                        InitDBDictionary();

                        //加载SQL语句
                        InitSqlModules(SQLMAPPING_FILEPATH, SQLMAPPING_FILEPATTERN);
                    }
                }
            }
        }

        /// <summary>
        /// 加载数据库连接串
        /// </summary>
        static void InitDBDictionary()
        {
            Dictionary<string, DataBase> dbMap = new Dictionary<string, DataBase>();

            SQLHelper sqlHelper = new SQLHelper();

            if (ConfigurationManager.ConnectionStrings[CONFIG_DBKEY] == null)
            {
                throw new Exception("请添加配置库至配置文件");
            }

            sqlHelper.ConnectionString = ConfigurationManager.ConnectionStrings[CONFIG_DBKEY].ConnectionString;

            string sql = INIT_CONNECTIONS_SQL;

            DataTable table = sqlHelper.ExecuteDataSet(sql, CommandType.Text, null).Tables[0];

            if (table != null)
            {
                foreach (DataRow dr in table.Rows)
                {
                    DataBase db = new DataBase();

                    db.DBKey = Convert.ToString(dr["DBKey"]);

                    db.ConnectionString = Convert.ToString(dr["ConnectionString"]);

                    dbMap[db.DBKey.ToLower()] = db;

                }
            }

            DBMap = dbMap;
        }

        /// <summary>
        /// 加载SqlMap文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="filePattern"></param>
        private static void InitSqlModules(string filePath, string filePattern)
        {
            Dictionary<string, ObjectSql> sqlMap = new Dictionary<string, ObjectSql>(100);

            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            if (basePath.EndsWith(@"\") == false)
            {
                basePath = basePath + @"\";
            }

            string sqlMapFolderPath = string.Format("{0}{1}", basePath, filePath);

            //如果没有SQLMap路径，不加载
            if (!Directory.Exists(sqlMapFolderPath))
            {
                return;
            }




            var sqlFiles = Directory.GetFiles(sqlMapFolderPath, SQLMAPPING_FILEPATTERN, SearchOption.AllDirectories);

            foreach (var sqlFile in sqlFiles)
            {
                var sqlMapXml = new XmlDocument();

                sqlMapXml.Load(sqlFile);

                var nsmgr = new XmlNamespaceManager(sqlMapXml.NameTable);

                nsmgr.AddNamespace("sm", "http://ZhaoPin.HighEnd.Serivice/SQLMapping/");

                var moduleNodes = sqlMapXml.DocumentElement.SelectNodes("sm:Module", nsmgr);

                foreach (XmlNode moduleNode in moduleNodes)
                {
                    #region Module

                    var moduleName = moduleNode.Attributes["ModuleName"].InnerText;

                    var dbKey = moduleNode.Attributes["DBKey"].InnerText;

                    var sqlMapElementes = moduleNode.SelectNodes("sm:ObjectSql", nsmgr);

                    foreach (XmlNode element in sqlMapElementes)
                    {
                        #region ObjectSql

                        var objectSql = new ObjectSql();

                        objectSql.SQLName = string.Format("{0}.{1}.{2}", dbKey, moduleName, element.Attributes["SqlName"].InnerText);

                        objectSql.SQLText = element.InnerText;

                        objectSql.DBKey = dbKey;

                        XmlNode node = element.Attributes.GetNamedItem("CommandType");

                        if (node != null)
                        {
                            objectSql.CommandType = (CommandType)Enum.Parse(typeof(CommandType), node.InnerText, true);
                        }

                        node = element.Attributes.GetNamedItem("OrderBy");

                        if (node != null)
                        {
                            objectSql.OrderBy = node.InnerText;
                        }

                        node = element.Attributes.GetNamedItem("Timeout");

                        if (node != null)
                        {
                            objectSql.CommandTimeout = Convert.ToInt32(node.InnerText);
                        }

                        sqlMap[objectSql.SQLName.ToLower()] = objectSql;

                        #endregion
                    }

                    #endregion
                }
            }

            SqlMap = sqlMap;
        }


        #endregion

        #region 私有方法

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramDic"></param>
        /// <param name="accesser"></param>
        /// <returns></returns>
        public static SqlParameter[] Transfrom(IDictionary<string, object> paramDic)
        {

            SqlParameter[] dbParams = null;

            if (null != paramDic && paramDic.Count > 0)
            {
                dbParams = new SqlParameter[paramDic.Count];

                int i = 0;

                foreach (var pair in paramDic)
                {
                    SqlParameter parameter = null;

                    if (pair.Value != null)
                    {
                        parameter = new SqlParameter(pair.Key, pair.Value);
                    }
                    else
                    {
                        parameter = new SqlParameter(pair.Key, DBNull.Value);
                    }


                    //parameter.s

                    dbParams[i] = parameter;

                    i++;
                }
            }

            return dbParams;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static SqlParameter MappingSqlPamameter(KeyValuePair<string, object> parameter)
        {
            SqlParameter sqlParameter = new SqlParameter();

            sqlParameter.ParameterName = parameter.Key;

            sqlParameter.SqlValue = parameter.Value;

            Type t = parameter.Value.GetType();

            #region 字段映射

            if (t.Equals(typeof(bool)))
            {
                sqlParameter.SqlDbType = SqlDbType.Bit;
            }
            else if (t.Equals(typeof(byte)))
            {
                sqlParameter.SqlDbType = SqlDbType.TinyInt;
            }
            else if (t.Equals(typeof(short)))
            {
                sqlParameter.SqlDbType = SqlDbType.SmallInt;
            }
            else if (t.Equals(typeof(int)))
            {
                sqlParameter.SqlDbType = SqlDbType.Int;
            }
            else if (t.Equals(typeof(long)))
            {
                sqlParameter.SqlDbType = SqlDbType.BigInt;
            }
            else if (t.Equals(typeof(decimal)))
            {
                sqlParameter.SqlDbType = SqlDbType.Decimal;
            }
            else if (t.Equals(typeof(float)))
            {
                sqlParameter.SqlDbType = SqlDbType.Float;
            }
            else if (t.Equals(typeof(double)))
            {
                sqlParameter.SqlDbType = SqlDbType.Float;
            }
            else if (t.Equals(typeof(string)))
            {
                string sV = parameter.Value.ToString();

                if (sV.Length <= 4000)
                {
                    sqlParameter.SqlDbType = SqlDbType.NVarChar;

                    sqlParameter.Size = sV.Length;
                }
                else
                {
                    sqlParameter.SqlDbType = SqlDbType.NText;
                }
            }
            else if (t.Equals(typeof(byte[])))
            {
                byte[] bt = (byte[])parameter.Value;

                sqlParameter.SqlDbType = SqlDbType.VarBinary;

                sqlParameter.Size = bt.Length;
            }
            else
            {
                sqlParameter = new SqlParameter(parameter.Key, parameter.Value);
            }

            #endregion

            return sqlParameter;

        }

        #endregion

        #region 公共调用方法

        /// <summary>
        /// 根据配置节名称查询当前业务连接的数据库
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static DataBase GetDatabase(string dbKey)
        {
            if (DBMap.ContainsKey(dbKey.ToLower()) == false)
            {
                throw new ArgumentOutOfRangeException("DBKey", dbKey, string.Format("没有定义数据库{0}", dbKey));
            }

            var db = DBMap[dbKey.ToLower()];

            return db;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbKey"></param>
        /// <returns></returns>
        public static SQLHelper GetDataAccess(string dbKey)
        {
            DataBase db = GetDatabase(dbKey);

            SQLHelper helper = new SQLHelper();

            helper.ConnectionString = db.ConnectionString;

            return helper;

        }

        /// <summary>
        /// 获取ObjectSql的定义
        /// </summary>
        /// <param name="sqlName"></param>
        /// <returns></returns>
        public static ObjectSql GetObjectSql(string sqlName)
        {
            var sqlKey = sqlName.ToLower();

            ObjectSql oSql = SqlMap[sqlKey];

            if (oSql == null)
            {
                throw new Exception("找不到需要执行的SQL配置");
            }

            return oSql;
        }

        #endregion

        #region ExecuteDataTable

        /// <summary>
        /// 执行指定SQL语句，返回查询数据
        /// </summary>
        /// <param name="dbKey"></param>
        /// <param name="sqlText"></param>
        /// <param name="paramDic"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string dbKey, string sqlText, IDictionary<string, object> paramDic)
        {
            return ExecuteDataTable(dbKey, sqlText, paramDic, DEFAULT_EXECUTE_SQL_TIMEOUT);
        }

        /// <summary>
        /// 执行指定SQL语句，返回查询数据
        /// </summary>
        /// <param name="dbKey"></param>
        /// <param name="sqlText"></param>
        /// <param name="paramDic"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string dbKey, string sqlText, IDictionary<string, object> paramDic, int commandTimeout)
        {
            return ExecuteDataTable(dbKey, sqlText, paramDic, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// 执行指定SQL语句，返回查询数据
        /// </summary>
        /// <param name="dbKey"></param>
        /// <param name="sqlText"></param>
        /// <param name="paramDic"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="partitionName"></param>
        /// <param name="partitionValues"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string dbKey, string sqlText, IDictionary<string, object> paramDic, int commandTimeout, CommandType commondType)
        {
            DataTable table = null;

            DataSet ds = ExecuteDataSet(dbKey, sqlText, paramDic, commandTimeout, commondType);

            table = ds.Tables[0];

            return table;
        }

        #endregion

        #region ExecuteDataSet Methods

        /// <summary>
        /// 通过指定SQL名称获取SQL语句，执行后返回查询数据
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="paramDic"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string sqlName, IDictionary<string, object> paramDic)
        {
            ObjectSql sql = GetObjectSql(sqlName);

            return ExecuteDataSet(sql.DBKey, sql.SQLText, paramDic, sql.CommandTimeout, sql.CommandType);
        }

        /// <summary>
        /// 执行指定SQL语句，返回查询数据
        /// </summary>
        /// <param name="dbKey"></param>
        /// <param name="sqlText"></param>
        /// <param name="paramDic"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string dbKey, string sqlText, IDictionary<string, object> paramDic)
        {
            return ExecuteDataSet(dbKey, sqlText, paramDic, DEFAULT_EXECUTE_SQL_TIMEOUT);
        }

        /// <summary>
        /// 执行指定SQL语句，返回查询数据
        /// </summary>
        /// <param name="dbKey"></param>
        /// <param name="sqlText"></param>
        /// <param name="paramDic"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string dbKey, string sqlText, IDictionary<string, object> paramDic, int commandTimeout)
        {
            return ExecuteDataSet(dbKey, sqlText, paramDic, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// 执行指定SQL语句，返回查询数据
        /// </summary>
        /// <param name="dbKey"></param>
        /// <param name="sqlText"></param>
        /// <param name="paramDic"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="partitionName"></param>
        /// <param name="partitionValues"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string dbKey, string sqlText, IDictionary<string, object> paramDic, int commandTimeout, CommandType commondType)
        {
            SQLHelper accesser = GetDataAccess(dbKey);

            accesser.TimeOut = commandTimeout;

            if (commandTimeout >= 0)
            {
                accesser.TimeOut = commandTimeout;
            }

            SqlParameter[] dbParams = Transfrom(paramDic);

            DataSet ds = accesser.ExecuteDataSet(sqlText, commondType, dbParams);

            return ds;
        }

        #endregion

        #region ExecuteScalar Methods


        /// <summary>
        /// 通过指定SQL名称获取SQL语句，执行后返回单值
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="paramDic"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string sqlName, IDictionary<string, object> paramDic)
        {
            ObjectSql sql = GetObjectSql(sqlName);

            return ExecuteScalar(sql.DBKey, sql.SQLText, paramDic, sql.CommandType, sql.CommandTimeout);
        }


        /// <summary>
        /// 执行指定SQL语句，返回单值
        /// </summary>
        /// <param name="dbKey"></param>
        /// <param name="sqlText"></param>
        /// <param name="paramDic"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string dbKey, string sqlText, IDictionary<string, object> paramDic)
        {
            return ExecuteScalar(dbKey, sqlText, paramDic, CommandType.Text);
        }


        /// <summary>
        /// 执行指定SQL语句，返回单值
        /// </summary>
        /// <param name="dbKey"></param>
        /// <param name="sqlText"></param>
        /// <param name="paramDic"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string dbKey, string sqlText, IDictionary<string, object> paramDic, CommandType commandType)
        {
            return ExecuteScalar(dbKey, sqlText, paramDic, commandType, DEFAULT_EXECUTE_SQL_TIMEOUT);
        }

        /// <summary>
        /// 执行指定SQL语句，返回单值
        /// </summary>
        /// <param name="dbKey"></param>
        /// <param name="sqlText"></param>
        /// <param name="paramDic"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string dbKey, string sqlText, IDictionary<string, object> paramDic, CommandType commandType, int commandTimeout)
        {
            object result = null;

            SQLHelper sqlHelper = GetDataAccess(dbKey);

            sqlHelper.TimeOut = commandTimeout;

            SqlParameter[] parameters = Transfrom(paramDic);

            result = sqlHelper.ExecuteScalar(sqlText, commandType, parameters);

            return result;


        }

        #endregion

        #region ExecuteNonQuery Methods

        /// <summary>
        ///  通过指定SQL名称获取SQL语句，执行后返回受影响的行数
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="paramDic"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string sqlName, IDictionary<string, object> paramDic)
        {
            ObjectSql sql = GetObjectSql(sqlName);

            return ExecuteNonQuery(sql.DBKey, sql.SQLText, paramDic, sql.CommandType, sql.CommandTimeout);
        }


        /// <summary>
        /// 执行指定SQL语句，返回受影响的行数
        /// </summary>
        /// <param name="dbKey"></param>
        /// <param name="sqlText"></param>
        /// <param name="paramDic"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string dbKey, string sqlText, IDictionary<string, object> paramDic)
        {
            return ExecuteNonQuery(dbKey, sqlText, paramDic, CommandType.Text);
        }

        /// <summary>
        /// 执行指定SQL语句，返回受影响的行数
        /// </summary>
        /// <param name="dbKey"></param>
        /// <param name="sqlText"></param>
        /// <param name="paramDic"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string dbKey, string sqlText, IDictionary<string, object> paramDic, CommandType commandType)
        {
            return ExecuteNonQuery(dbKey, sqlText, paramDic, commandType, DEFAULT_EXECUTE_SQL_TIMEOUT);
        }

        /// <summary>
        /// 执行指定SQL语句，返回受影响的行数
        /// </summary>
        /// <param name="dbKey"></param>
        /// <param name="sqlText"></param>
        /// <param name="paramDic"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string dbKey, string sqlText, IDictionary<string, object> paramDic, CommandType commandType, int commandTimeout)
        {
            SQLHelper sqlHelper = GetDataAccess(dbKey);

            sqlHelper.TimeOut = commandTimeout;

            SqlParameter[] parameters = Transfrom(paramDic);

            return sqlHelper.ExecuteNonQuery(sqlText, commandType, parameters);
        }

        /// <summary>
        /// 执行BulkCopy, 用于大量写入数据
        /// </summary>
        /// <param name="destTableName">目标表</param>
        /// <param name="copyData">待写入数据的DataTable</param>
        /// <param name="columns">对应的列名</param>
        public static void ExecuteBulkCopyFromDt(string dbKey, string destTableName, DataTable copyData, string[][] columns)
        {
            SQLHelper sqlHelper = GetDataAccess(dbKey);

            sqlHelper.TimeOut = DEFAULT_EXECUTE_SQL_TIMEOUT;

            sqlHelper.ExecuteBulkCopyFromDt(destTableName, copyData, columns);
        }

        #endregion

        #region ExcuteDBReader Methods


        /// <summary>
        ///  通过指定SQL名称获取SQL语句，执行后返回受影响的行数
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="paramDic"></param>
        /// <returns></returns>
        public static DbDataReader ExecuteReader(string sqlName, IDictionary<string, object> paramDic)
        {
            ObjectSql sql = GetObjectSql(sqlName);

            return ExecuteReader(sql.DBKey, sql.SQLText, paramDic, sql.CommandType, sql.CommandTimeout);
        }


        /// <summary>
        /// 执行指定SQL语句，返回受影响的行数
        /// </summary>
        /// <param name="dbKey"></param>
        /// <param name="sqlText"></param>
        /// <param name="paramDic"></param>
        /// <returns></returns>
        public static DbDataReader ExecuteReader(string dbKey, string sqlText, IDictionary<string, object> paramDic)
        {
            return ExecuteReader(dbKey, sqlText, paramDic, CommandType.Text);
        }

        /// <summary>
        /// 执行指定SQL语句，返回受影响的行数
        /// </summary>
        /// <param name="dbKey"></param>
        /// <param name="sqlText"></param>
        /// <param name="paramDic"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static DbDataReader ExecuteReader(string dbKey, string sqlText, IDictionary<string, object> paramDic, CommandType commandType)
        {
            return ExecuteReader(dbKey, sqlText, paramDic, commandType, DEFAULT_EXECUTE_SQL_TIMEOUT);
        }

        /// <summary>
        /// 执行指定SQL语句，返回受影响的行数
        /// </summary>
        /// <param name="dbKey"></param>
        /// <param name="sqlText"></param>
        /// <param name="paramDic"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static DbDataReader ExecuteReader(string dbKey, string sqlText, IDictionary<string, object> paramDic, CommandType commandType, int commandTimeout)
        {
            SQLHelper sqlHelper = GetDataAccess(dbKey);

            sqlHelper.TimeOut = commandTimeout;

            SqlParameter[] parameters = Transfrom(paramDic);

            return sqlHelper.ExecuteReader(sqlText, commandType, parameters);
        }


        #endregion

        #region ExecuteTableForPage

        /// <summary>
        /// 通过指定SQL名称获取SQL语句，执行后返回受影响的行数
        /// </summary>
        /// <param name="sqlName"></param>
        /// <param name="paramDic"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="itemCount"></param>
        /// <param name="dbParameterList"></param>
        /// <param name="orderbyPart"></param>
        /// <returns></returns>
        public static DataTable ExecuteReader(string sqlName, IDictionary<string, object> paramDic, int pageIndex, int pageSize, out int itemCount, SqlParameter[] dbParameterList, string orderbyPart)
        {
            ObjectSql sql = GetObjectSql(sqlName);

            return ExecuteTableForPage(sql.DBKey, sql.SQLText, paramDic, pageIndex, pageSize, out itemCount, sql.OrderBy);
        }

        /// <summary>
        /// 按照SQL语句和Order by部分进行分页查询的方法
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示条数</param>
        /// <param name="itemCount">总记录数</param>
        /// <param name="sql">SQL语句，不能带order by 部分</param>
        /// <param name="dbParameterList">参数</param>
        /// <param name="orderbyPart">sql语句的orderby部分如： adddate desc,userid 或者 a.adddate desc,b.userid 不能带(order by)</param>
        /// <returns></returns>
        public static DataTable ExecuteTableForPage(string dbKey, string sqlText, IDictionary<string, object> paramDic, int pageIndex, int pageSize, out int itemCount, string orderbyPart)
        {
            return ExecuteTableForPage(dbKey, sqlText, paramDic, DEFAULT_EXECUTE_SQL_TIMEOUT, pageIndex, pageSize, out itemCount, orderbyPart);
        }

        /// <summary>
        /// 按照SQL语句和Order by部分进行分页查询的方法
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示条数</param>
        /// <param name="itemCount">总记录数</param>
        /// <param name="sql">SQL语句，不能带order by 部分</param>
        /// <param name="dbParameterList">参数</param>
        /// <param name="orderbyPart">sql语句的orderby部分如： adddate desc,userid 或者 a.adddate desc,b.userid 不能带(order by)</param>
        /// <returns></returns>
        public static DataTable ExecuteTableForPage(string dbKey, string sqlText, IDictionary<string, object> paramDic, int pageIndex, int pageSize, out int itemCount, string orderbyPart, int timeOut)
        {
            return ExecuteTableForPage(dbKey, sqlText, paramDic, timeOut, pageIndex, pageSize, out itemCount, orderbyPart);
        }

        /// <summary>
        /// 按照SQL语句和Order by部分进行分页查询的方法
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示条数</param>
        /// <param name="itemCount">总记录数</param>
        /// <param name="sql">SQL语句，不能带order by 部分</param>
        /// <param name="dbParameterList">参数</param>
        /// <param name="orderbyPart">sql语句的orderby部分如： adddate desc,userid 或者 a.adddate desc,b.userid 不能带(order by)</param>
        /// <returns></returns>
        public static DataTable ExecuteTableForPage(string dbKey, string sqlText, IDictionary<string, object> paramDic, int commandTimeout, int pageIndex, int pageSize, out int itemCount, string orderbyPart)
        {
            SQLHelper sqlHelper = GetDataAccess(dbKey);

            sqlHelper.TimeOut = commandTimeout;

            SqlParameter[] parameters = Transfrom(paramDic);

            return sqlHelper.ExecuteTableForPage(pageIndex, pageSize, out itemCount, sqlText, parameters, orderbyPart);
        }


        #endregion

    }
}
