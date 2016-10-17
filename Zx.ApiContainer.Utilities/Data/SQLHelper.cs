using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Zx.ApiContainer.Utilities.Data
{
    /// <summary> 
    /// 自定义访问通用类 
    /// </summary> 
    public class SQLHelper
    {
        #region 配置属性

        /// <summary>
        /// 连接串
        /// </summary>
        public string ConnectionString
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int TimeOut
        {
            get;
            set;
        }

        /// <summary> 
        /// 构造函数 
        /// </summary> 
        public SQLHelper()
        {
            this.TimeOut = 30;
        }

        /// <summary>
        /// 批量增加数据的数量
        /// </summary>
        private const int _bulkCopySize = 50000;

        #endregion

        #region 公共方法

        /// <summary> 
        /// ExecuteNonQuery操作，对数据库进行 增、删、改 操作（3） 
        /// </summary> 
        /// <param name="sql">要执行的SQL语句 </param> 
        /// <param name="commandType">要执行的查询类型（存储过程、SQL文本） </param> 
        /// <param name="parameters">参数数组 </param> 
        /// <returns> </returns> 
        public virtual int ExecuteNonQuery(string sql, CommandType commandType, SqlParameter[] parameters)
        {
            int count = 0;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = commandType;

                    command.CommandTimeout = this.TimeOut;

                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    connection.Open();

                    count = command.ExecuteNonQuery();
                }
            }
            return count;
        }

        /// <summary> 
        /// SqlDataAdapter的Fill方法执行一个查询，并返回一个DataSet类型结果（3） 
        /// </summary> 
        /// <param name="sql">要执行的SQL语句 </param> 
        /// <param name="commandType">要执行的查询类型（存储过程、SQL文本） </param> 
        /// <param name="parameters">参数数组 </param> 
        /// <returns> </returns> 
        public virtual DataSet ExecuteDataSet(string sql, CommandType commandType, SqlParameter[] parameters)
        {
            DataSet ds = new DataSet();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = commandType;

                    command.CommandTimeout = this.TimeOut;

                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(command);

                    adapter.Fill(ds);
                }
            }
            return ds;
        }


        /// <summary> 
        /// ExecuteReader执行一查询，返回一SqlDataReader对象实例（3） 
        /// </summary> 
        /// <param name="sql">要执行的SQL语句 </param> 
        /// <param name="commandType">要执行的查询类型（存储过程、SQL文本） </param> 
        /// <param name="parameters">参数数组 </param> 
        /// <returns> </returns> 
        public virtual SqlDataReader ExecuteReader(string sql, CommandType commandType, SqlParameter[] parameters)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);

            SqlCommand command = new SqlCommand(sql, connection);

            command.CommandType = commandType;

            command.CommandTimeout = this.TimeOut;

            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }
            connection.Open();

            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        /// <summary> 
        /// ExecuteScalar执行一查询，返回查询结果的第一行第一列（3） 
        /// </summary> 
        /// <param name="sql">要执行的SQL语句 </param> 
        /// <param name="commandType">要执行的查询类型（存储过程、SQL文本） </param> 
        /// <returns> </returns> 
        public virtual object ExecuteScalar(string sql, CommandType commandType, SqlParameter[] parameters)
        {
            object result = null;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = commandType;

                    command.CommandTimeout = this.TimeOut;

                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    connection.Open();

                    result = command.ExecuteScalar();
                }
            }
            return result;
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
        public virtual DataTable ExecuteTableForPage(int pageIndex, int pageSize, out int itemCount, string sql, SqlParameter[] dbParameterList, string orderbyPart)
        {
            if (orderbyPart == String.Empty)
            {
                throw new Exception("order by不能为空!");
            }

            string fromPart = " from (" + sql + ")  a ";
            string sqlCount = "select count(*)  " + fromPart;
            StringBuilder sbSelect = new StringBuilder(sqlCount);
            sbSelect.Append(";");
            sbSelect.Append(this.GetSelectTopSql(pageIndex, pageSize, sql, "*", orderbyPart));


            DataSet ds = this.ExecuteDataSet(sbSelect.ToString(), CommandType.Text, dbParameterList);
            itemCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            DataTable table = ds.Tables[1];
            return table;
        }

        /// <summary>
        /// 执行BulkCopy, 用于大量写入数据
        /// </summary>
        /// <param name="destTableName">目标表</param>
        /// <param name="copyData">待写入数据的DataTable</param>
        /// <param name="columns">对应的列名</param>
        public virtual void ExecuteBulkCopyFromDt(string destTableName, DataTable copyData, string[][] columns)
        {
            if (copyData == null || copyData.Rows.Count == 0)
            {
                return;
            }
            //throw new InvalidOperationException("NULL or Empty BulkCopy datas");

            SqlConnection connection = new SqlConnection(ConnectionString);

            connection.Open();

            SqlTransaction sqlbulkTransaction = connection.BeginTransaction();

            try
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.CheckConstraints, sqlbulkTransaction))
                {
                    bulkCopy.DestinationTableName = destTableName;
                    bulkCopy.BatchSize = copyData.Rows.Count > _bulkCopySize ? _bulkCopySize : copyData.Rows.Count;

                    if (columns != null && columns.Length > 1)
                    {
                        for (int i = 0; i < columns[0].Length; i++)
                        {
                            SqlBulkCopyColumnMapping scc = new SqlBulkCopyColumnMapping(columns[0][i].Trim(), columns[1][i].Trim());
                            bulkCopy.ColumnMappings.Add(scc);
                        }
                    }
                    bulkCopy.WriteToServer(copyData);

                    sqlbulkTransaction.Commit();
                }

            }
            catch (Exception ex)
            {
                sqlbulkTransaction.Rollback();

                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sql"></param>
        /// <param name="fileds"></param>
        /// <param name="orderbyPart"></param>
        /// <returns></returns>
        private string GetSelectTopSql(int pageIndex, int pageSize, string sql, string fileds, string orderbyPart)
        {
            string fromPart = " from (" + sql + ")  a ";
            orderbyPart = " order by " + orderbyPart;

            StringBuilder sbSelect = new StringBuilder();

            sbSelect.Append("select top ");
            sbSelect.Append(pageSize);
            sbSelect.Append(" ");
            sbSelect.Append(fileds);
            sbSelect.Append(" from (select ");
            sbSelect.Append(fileds);
            sbSelect.Append(",row_number() over(");
            sbSelect.Append(orderbyPart);
            sbSelect.Append(") as rownumber ");
            sbSelect.Append(fromPart);
            sbSelect.Append(") aa where rownumber>");
            sbSelect.Append((pageIndex - 1) * pageSize);
            sbSelect.Append(" order by rownumber");

            return sbSelect.ToString();
        }

        #endregion
    }
}
