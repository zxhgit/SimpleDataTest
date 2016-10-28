using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using Zx.ApiContainer.Entity;
using Zx.ApiContainer.Entity.Attributes;
using Zx.ApiContainer.Entity.Configration;
using Zx.ApiContainer.Services.DataAdapters;
using Zx.ApiContainer.Utilities;
using Zx.ApiContainer.Utilities.Data;
using Zx.ApiContainer.Wrappers.Cache;

namespace Zx.ApiContainer.Repository.DBAdapters
{
    public abstract class BasicDBAdapter : MarshalByRefObject, IBasicDataAdapter
    {
        #region Constructor

        /// <summary>
        /// 只读
        /// </summary>
        const string DB_KEY_READ = "_READ";

        public bool IsTag = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        public BasicDBAdapter(bool isRead = false)
        {
            DisableNullUpdate = DISABLE_NULL_UPDATE;

            IsTag = isRead;
        }

        #endregion

        #region Constants

        /// <summary>
        /// 默认禁止空值更新
        /// </summary>
        const bool DISABLE_NULL_UPDATE = true;

        #endregion

        #region 属性

        /// <summary>
        /// 是否禁用空值更新
        /// </summary>
        public bool DisableNullUpdate
        {
            get;
            set;
        }

        /// <summary>
        /// 是否异步
        /// </summary>
        public bool Asynchronous
        {
            get;
            set;
        }

        /// <summary>
        /// 是否开启数据缓存
        /// </summary>
        public bool EnableCache
        {
            get;
            set;
        }

        /// <summary>
        /// 只读库配置节
        /// </summary>
        protected virtual string DBKey_ReadOnly
        {
            get
            {
                return dbkey + DB_KEY_READ;
            }
        }

        /// <summary>
        /// 数据库配置名
        /// </summary>
        public virtual string DBKey
        {
            get
            {
                return dbkey;
            }
            set
            {
                if (IsTag)
                {
                    dbkey = value + DB_KEY_READ;
                }
                else
                {
                    dbkey = value;
                }
            }
        }

        /// <summary>
        /// 数据库配置名
        /// </summary>
        private string dbkey;

        #endregion

        #region 静态实例

        /// <summary>
        /// 缓存的类型实例
        /// </summary>
        static Dictionary<string, InstanceStoredEntity> _InstanceTypeDic = new Dictionary<string, InstanceStoredEntity>();

        /// <summary>
        /// 静态对象（用于锁）
        /// </summary>
        static object _LockPad = new object();

        #endregion

        #region 私有方法

        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="entity">实体</param>
        private void SetToCache<T>(T entity)
        {
            InstanceStoredEntity instanceType = this.GetInstanceMembers<T>();

            object primaryKey = instanceType.PrimaryKeyProperty.CurrentProperty.GetValue(entity);

            if (primaryKey == null)
            {
                throw new Exception("主键不能为空");
            }

            RedisOpt.SetSingleCacheObject<T>(primaryKey.ToString(), entity);
        }

        private void SetToCache<T>(object primaryKey, T entity)
        {
            if (primaryKey == null)
            {
                throw new Exception("主键不能为空");
            }

            RedisOpt.SetSingleCacheObject<T>(primaryKey.ToString(), entity);
        }

        /// <summary>
        /// 从缓存中读取数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="primaryKey">主键</param>
        /// <param name="instanceType">类型缓存对象</param>
        private T GetFromCache<T>(object primaryKey, InstanceStoredEntity instanceType)
        {
            return RedisOpt.GetSingleCacheObject<T>(primaryKey.ToString());
        }

        /// <summary>
        /// 从缓存中删除数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <typeparam name="T">类型</typeparam>
        private bool RemoveFromCache<T>(T entity)
        {
            if (!EnableCache)
            {
                return true;
            }

            InstanceStoredEntity instanceType = this.GetInstanceMembers<T>();

            object primaryKey = instanceType.PrimaryKeyProperty.CurrentProperty.GetValue(entity);

            if (primaryKey == null)
            {
                throw new Exception("主键不能为空");
            }

            bool b = RedisOpt.RemoveSingleCacheObject<T>(primaryKey.ToString());

            return b;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        private bool RemoveFromCacheByPrimaryKey<T>(object primaryKey)
        {
            if (!EnableCache)
            {
                return true;
            }

            if (primaryKey == null)
            {
                throw new Exception("主键不能为空");
            }

            bool b = RedisOpt.RemoveSingleCacheObject<T>(primaryKey.ToString());

            return b;
        }

        /// <summary>
        /// 执行某段sql脚本，返回一个数据集的泛型方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqltxt"></param>
        /// <param name="paramDic"></param>
        /// <returns></returns>
        protected List<T> GetObjectList<T>(string sqltxt, Dictionary<string, object> paramDic)
        {
            //获取数据集
            DataSet ds = DBAccessor.ExecuteDataSet(this.DBKey, sqltxt, paramDic);

            List<T> list = new List<T>();
            //循环数据表中的每一行
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                InstanceStoredEntity instanceType = GetInstanceMembers<T>();
                T show = this.MappingEntity<T>(dr, instanceType);
                list.Add(show);
            }
            return list;
        }

        /// <summary>
        /// 获取单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqltxt"></param>
        /// <param name="paramDic"></param>
        /// <returns></returns>
        protected T GetObject<T>(string sqltxt, Dictionary<string, object> paramDic)
        {
            T entity = default(T);

            InstanceStoredEntity instanceType = GetInstanceMembers<T>();
            //获取数据集
            DataTable table = DBAccessor.ExecuteDataTable(this.DBKey, sqltxt, paramDic);

            if (table.Rows.Count == 0)
            {
                return default(T);
            }

            DataRow dr = table.Rows[0];

            entity = MappingEntity<T>(dr, instanceType);

            return entity;
        }

        /// <summary>
        /// 执行sql脚本返回记录数量
        /// </summary>
        /// <param name="sqltxt"></param>
        /// <param name="paramDic"></param>
        /// <returns></returns>
        protected int GetObjectCount(string sqltxt, Dictionary<string, object> paramDic)
        {
            int count = (int)DBAccessor.ExecuteScalar(this.DBKey, sqltxt, paramDic);
            return count;
        }

        /// <summary>
        /// 将数据行映射到某个实体类型
        /// </summary>
        /// <param name="dr">数据行</param>
        /// <param name="instanceType">实体类型</param>
        /// <returns></returns>
        protected T MappingEntity<T>(DataRow dr, InstanceStoredEntity instanceType)
        {
            DataTable table = dr.Table;

            T entity = (T)EntityHelper.GetEntity(instanceType.InstanceType);

            //循环每一列
            foreach (DataColumn column in table.Columns)
            {
                //获取当前字段值
                object currentValue = dr[column];

                PropertyStoredEntity propertyType = instanceType.GetProperty(column.ColumnName);

                if (propertyType == null)
                {
                    continue;
                }

                if (currentValue is DBNull)
                {
                    //值类型 数据为空不赋值
                    if (!propertyType.CurrentProperty.PropertyType.IsValueType)
                    {
                        propertyType.CurrentProperty.SetValue(entity, null);
                    }
                }
                else
                {
                    if (propertyType != null)
                    {
                        try
                        {
                            //针对datetime类型的属性增加获取UTC格式日期的逻辑
                            string propertyTypeStr = GetPropertyTypeStr(propertyType.CurrentProperty);
                            if (propertyTypeStr.ToLower() == "datetime")
                            {
                                propertyType.CurrentProperty.SetValue(entity, DateTime.SpecifyKind((System.DateTime)currentValue, DateTimeKind.Utc));
                            }
                            else
                            {
                                propertyType.CurrentProperty.SetValue(entity, currentValue);
                            }

                            //propertyType.CurrentProperty.SetValue(entity, currentValue);
                        }
                        catch (Exception ex)
                        {
                            Exception exNew = new Exception(propertyType.CurrentProperty.Name, ex);
                            throw exNew;
                        }
                    }
                }

            }

            return entity;
        }


        /// <summary>
        /// 获取删除命令
        /// </summary>
        /// <param name="instanceType">表类型属性</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        private SQLExcutiveCommand GetDeleteCommand<T>(InstanceStoredEntity instanceType, T entity)
        {
            SQLExcutiveCommand executeCommand = new SQLExcutiveCommand();

            StringBuilder builder = new StringBuilder();

            StringBuilder columnsBuilder = new StringBuilder();

            builder.Append("Delete from " + instanceType.TableName + " where ");

            PropertyStoredEntity property = instanceType.PrimaryKeyProperty;

            //空值不执行数据库操作
            object rowValue = property.CurrentProperty.GetValue(entity);

            //SQL语句字段部分
            columnsBuilder.Append(property.CurrentProperty.Name + "=@" + property.CurrentProperty.Name);

            //参数值
            executeCommand.ParamDic.Add("@" + property.CurrentProperty.Name, rowValue);

            builder.Append(columnsBuilder);

            executeCommand.SQLText = builder.ToString();

            return executeCommand;
        }

        /// <summary>
        /// 根据主键获取删除命令
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        private SQLExcutiveCommand GetDeleteCommand(InstanceStoredEntity instanceType, object primaryKey)
        {
            SQLExcutiveCommand executeCommand = new SQLExcutiveCommand();

            StringBuilder builder = new StringBuilder();

            StringBuilder columnsBuilder = new StringBuilder();

            builder.Append("Delete from " + instanceType.TableName + " where ");

            PropertyStoredEntity property = instanceType.PrimaryKeyProperty;

            //SQL语句字段部分
            columnsBuilder.Append(property.CurrentProperty.Name + "=@" + property.CurrentProperty.Name);

            //参数值
            executeCommand.ParamDic.Add("@" + property.CurrentProperty.Name, primaryKey);

            builder.Append(columnsBuilder);

            executeCommand.SQLText = builder.ToString();

            return executeCommand;
        }

        /// <summary>
        /// 获取查询命令(头部分)
        /// </summary>
        /// <param name="instanceType">表类型属性</param>
        /// <param name="queriedColumns">需要的字段</param>
        /// <returns></returns>
        private string GetQueryCommandTitle(InstanceStoredEntity instanceType, List<string> queriedColumns)
        {

            StringBuilder builder = new StringBuilder();

            StringBuilder columnsBuilder = new StringBuilder();

            builder.Append("Select ");

            bool firstAppend = true;

            if (queriedColumns == null)
            {
                columnsBuilder.Append("*");
            }
            else
            {
                foreach (string column in queriedColumns)
                {
                    if (firstAppend)
                    {
                        columnsBuilder.Append(column);
                        firstAppend = false;
                    }
                    else
                    {
                        columnsBuilder.Append("," + column);
                    }
                }
            }

            builder.Append(columnsBuilder);

            builder.Append(" from " + instanceType.TableName);

            return builder.ToString();
        }

        /// <summary>
        /// 获取查询命令
        /// </summary>
        /// <param name="instanceType">表类型属性</param>
        /// <param name="entity">实体</param>
        /// <param name="queriedColumns">需要的字段</param>
        /// <returns></returns>
        private SQLExcutiveCommand GetQueryCommand<T>(InstanceStoredEntity instanceType, object primaryKey, List<string> queriedColumns)
        {
            SQLExcutiveCommand executeCommand = new SQLExcutiveCommand();

            StringBuilder builder = new StringBuilder();

            string queryTitle = GetQueryCommandTitle(instanceType, queriedColumns);

            builder.Append(queryTitle);

            builder.Append(" where ");

            builder.Append(instanceType.PrimaryKeyProperty.CurrentProperty.Name + "=@" + instanceType.PrimaryKeyProperty.CurrentProperty.Name);

            executeCommand.ParamDic.Add("@" + instanceType.PrimaryKeyProperty.CurrentProperty.Name, primaryKey);

            executeCommand.SQLText = builder.ToString();

            return executeCommand;
        }

        /// <summary>
        /// 获取添加命令
        /// </summary>
        /// <param name="instanceType">表类型属性</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        protected virtual SQLExcutiveCommand GetInsertCommand<T>(InstanceStoredEntity instanceType, T entity)
        {
            SQLExcutiveCommand executeCommand = new SQLExcutiveCommand();

            StringBuilder builder = new StringBuilder();

            StringBuilder columnsBuilder = new StringBuilder();

            StringBuilder paramsBuilder = new StringBuilder();

            builder.Append("Insert into " + instanceType.TableName + " ");

            bool firstAppend = true;

            columnsBuilder.Append("(");

            paramsBuilder.Append(" values (");


            foreach (KeyValuePair<string, PropertyStoredEntity> pair in instanceType.Properties)
            {

                PropertyStoredEntity property = pair.Value;

                #region 循环拼接字段部分和参数部分

                //空值不执行数据库操作
                object rowValue = property.CurrentProperty.GetValue(entity);

                if (rowValue == null || property.IsIdentity)
                {
                    continue;
                }

                //if (property.IsMultiLine)
                //{
                //    rowValue = rowValue.ToString().Replace("\r", string.Empty);
                //}

                if (firstAppend)
                {
                    firstAppend = false;
                }
                else
                {
                    columnsBuilder.Append(",");

                    paramsBuilder.Append(",");
                }

                //SQL语句字段部分
                columnsBuilder.Append(property.CurrentProperty.Name);

                //SQL语句参数部分
                paramsBuilder.Append("@" + property.CurrentProperty.Name);

                //参数值
                executeCommand.ParamDic.Add("@" + property.CurrentProperty.Name, rowValue);


                #endregion
            }

            columnsBuilder.Append(") ");

            paramsBuilder.Append(")");

            builder.Append(columnsBuilder);

            builder.Append(paramsBuilder);

            if (instanceType.IdentityProperty != null)
            {


                string sqlType = DataTypeMapping.GetSqlTypeString(instanceType.IdentityProperty.DataType);

                builder.Append(";Select cast(@@Identity as " + sqlType + ")");
            }

            executeCommand.SQLText = builder.ToString();

            return executeCommand;
        }

        /// <summary>
        /// 获取添加命令
        /// </summary>
        /// <param name="instanceType">表类型属性</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        private SQLExcutiveCommand GetIdentityInsertCommand<T>(InstanceStoredEntity instanceType, T entity)
        {
            SQLExcutiveCommand executeCommand = new SQLExcutiveCommand();

            StringBuilder builder = new StringBuilder();

            StringBuilder columnsBuilder = new StringBuilder();

            StringBuilder paramsBuilder = new StringBuilder();

            builder.Append("SET IDENTITY_INSERT " + instanceType.TableName + " ON;Insert into " + instanceType.TableName + " ");

            bool firstAppend = true;

            columnsBuilder.Append("(");

            paramsBuilder.Append(" values (");


            foreach (KeyValuePair<string, PropertyStoredEntity> pair in instanceType.Properties)
            {

                PropertyStoredEntity property = pair.Value;

                #region 循环拼接字段部分和参数部分

                //空值不执行数据库操作
                object rowValue = property.CurrentProperty.GetValue(entity);

                if (rowValue == null)
                {
                    continue;
                }

                //if (property.IsMultiLine)
                //{
                //    rowValue = rowValue.ToString().Replace("\r", string.Empty);
                //}

                if (firstAppend)
                {
                    firstAppend = false;
                }
                else
                {
                    columnsBuilder.Append(",");

                    paramsBuilder.Append(",");
                }

                //SQL语句字段部分
                columnsBuilder.Append(property.CurrentProperty.Name);

                //SQL语句参数部分
                paramsBuilder.Append("@" + property.CurrentProperty.Name);

                //参数值
                executeCommand.ParamDic.Add("@" + property.CurrentProperty.Name, rowValue);


                #endregion
            }

            columnsBuilder.Append(")");

            paramsBuilder.Append(")");

            builder.Append(columnsBuilder);

            builder.Append(paramsBuilder);

            builder.Append(";SET IDENTITY_INSERT " + instanceType.TableName + " Off;");

            executeCommand.SQLText = builder.ToString();

            return executeCommand;
        }

        /// <summary>
        /// 获取更新命令
        /// </summary>
        /// <param name="instanceType">表类型属性</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        private SQLExcutiveCommand GetUpdateCommand<T>(InstanceStoredEntity instanceType, T entity)
        {
            SQLExcutiveCommand executeCommand = new SQLExcutiveCommand();

            StringBuilder builder = new StringBuilder();

            StringBuilder columnsBuilder = new StringBuilder();

            builder.Append("Update " + instanceType.TableName + " Set ");

            bool firstAppend = true;

            StringBuilder wherePart = new StringBuilder();


            foreach (KeyValuePair<string, PropertyStoredEntity> pair in instanceType.Properties)
            {

                PropertyStoredEntity property = pair.Value;

                #region 循环拼接字段部分和参数部分

                //空值不执行数据库操作
                object rowValue = property.CurrentProperty.GetValue(entity);

                if (property.IsPrimaryKey)
                {
                    wherePart.Append(" where " + property.CurrentProperty.Name + "=@" + property.CurrentProperty.Name);

                    executeCommand.ParamDic.Add("@" + property.CurrentProperty.Name, rowValue);

                    continue;
                }

                if (this.DisableNullUpdate)
                {
                    if (rowValue == null)
                    {
                        continue;
                    }
                }

                if (property.IsIdentity)
                {
                    continue;
                }

                //if (property.IsMultiLine)
                //{
                //    rowValue = rowValue.ToString().Replace("\r", string.Empty);
                //}

                if (firstAppend)
                {
                    firstAppend = false;
                }
                else
                {
                    columnsBuilder.Append(",");
                }

                //SQL语句字段部分
                columnsBuilder.Append(property.CurrentProperty.Name + "=@" + property.CurrentProperty.Name);

                if (rowValue == null)
                {
                    //参数值
                    executeCommand.ParamDic.Add("@" + property.CurrentProperty.Name, DBNull.Value);
                }
                else
                {
                    //参数值
                    executeCommand.ParamDic.Add("@" + property.CurrentProperty.Name, rowValue);
                }




                #endregion
            }

            builder.Append(columnsBuilder);

            builder.Append(wherePart);

            executeCommand.SQLText = builder.ToString();

            return executeCommand;
        }

        /// <summary>
        /// 获取主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        private object GetPrimaryKey<T>(T entity)
        {
            InstanceStoredEntity instanceType = GetInstanceMembers<T>();

            if (instanceType.PrimaryKeyProperty == null)
            {
                new Exception("主键不能为空");
            }

            object primaryKey = instanceType.PrimaryKeyProperty.CurrentProperty.GetValue(entity);

            return primaryKey;
        }

        /// <summary>
        /// 获取实体类型
        /// </summary>
        /// <param name="entity">实体</param>
        protected InstanceStoredEntity GetInstanceMembersByObj(object o)
        {
            //获取泛型的类型
            Type instanceType = o.GetType();

            if (!_InstanceTypeDic.ContainsKey(instanceType.FullName))
            {
                lock (_LockPad)
                {
                    if (!_InstanceTypeDic.ContainsKey(instanceType.FullName))
                    {
                        InstanceStoredEntity instanceEntity = new InstanceStoredEntity();

                        instanceEntity.TableName = instanceType.Name;

                        instanceEntity.InstanceType = instanceType;

                        //为当前实例的属性组new一个字典
                        instanceEntity.Properties = new Dictionary<string, PropertyStoredEntity>();

                        //获取实例类型的public属性
                        PropertyInfo[] properties = instanceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                        #region 循环加载Property属性
                        foreach (PropertyInfo property in properties)
                        {

                            #region 判断当前属性，并以自定义的属性类型的方式存入实体实例的属性组中
                            //判断当前属性是否具有自定义的属性（非数据表字段）
                            object c = property.GetCustomAttribute(typeof(DBIngoreAttribute));

                            //当前属性上未获取自定义的DBIngoreAttribute属性，
                            //表示当前是数据库的表字段,则继续执行{}内的代码
                            if (c == null)
                            {
                                PropertyStoredEntity cache = new PropertyStoredEntity();

                                cache.CurrentProperty = property;

                                //判断当前属性是否具有自定义的属性（数据表字段用于唯一标识）
                                c = property.GetCustomAttribute(typeof(IdentityAttribute));
                                if (c != null)
                                {
                                    cache.IsIdentity = true;
                                }

                                //判断当前属性是否具有自定义的属性（数据表字段为主键）
                                c = property.GetCustomAttribute(typeof(PrimaryKeyAttribute));
                                if (c != null)
                                {
                                    cache.IsPrimaryKey = true;
                                }

                                //c = property.GetCustomAttribute(typeof(MultiLineAttribute));
                                //if (c != null)
                                //{
                                //    cache.IsMultiLine = true;
                                //}

                                cache.DataType = property.PropertyType.Name;

                                //将当前的属性，以自定义类型的方式存入实体实例的属性组中
                                instanceEntity.Properties.Add(property.Name.ToLower(), cache);

                                if (cache.IsPrimaryKey)
                                {
                                    instanceEntity.PrimaryKeyProperty = cache;
                                }

                                if (cache.IsIdentity)
                                {
                                    instanceEntity.IdentityProperty = cache;
                                }

                                //if (cache.IsMultiLine)
                                //{
                                //    instanceEntity.MultiLineProperty = cache;
                                //}
                            }
                            #endregion

                        }
                        #endregion

                        //将实例缓存在字典中
                        _InstanceTypeDic.Add(instanceType.FullName, instanceEntity);
                    }
                }
            }

            //将缓存中的实例返回
            return _InstanceTypeDic[instanceType.FullName];
        }

        /// <summary>
        /// 获取实体类型
        /// </summary>
        /// <param name="entity">实体</param>
        protected InstanceStoredEntity GetInstanceMembers<T>()
        {
            //获取泛型的类型
            Type instanceType = typeof(T);

            if (!_InstanceTypeDic.ContainsKey(instanceType.FullName))
            {
                lock (_LockPad)
                {
                    if (!_InstanceTypeDic.ContainsKey(instanceType.FullName))
                    {
                        InstanceStoredEntity instanceEntity = new InstanceStoredEntity();

                        instanceEntity.TableName = instanceType.Name;

                        instanceEntity.InstanceType = instanceType;

                        //为当前实例的属性组new一个字典
                        instanceEntity.Properties = new Dictionary<string, PropertyStoredEntity>();

                        //获取实例类型的public属性
                        PropertyInfo[] properties = instanceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                        #region 循环加载Property属性
                        foreach (PropertyInfo property in properties)
                        {

                            #region 判断当前属性，并以自定义的属性类型的方式存入实体实例的属性组中
                            //判断当前属性是否具有自定义的属性（非数据表字段）
                            object c = property.GetCustomAttribute(typeof(DBIngoreAttribute));

                            //当前属性上未获取自定义的DBIngoreAttribute属性，
                            //表示当前是数据库的表字段,则继续执行{}内的代码
                            if (c == null)
                            {
                                PropertyStoredEntity cache = new PropertyStoredEntity();

                                cache.CurrentProperty = property;

                                //判断当前属性是否具有自定义的属性（数据表字段用于唯一标识）
                                c = property.GetCustomAttribute(typeof(IdentityAttribute));
                                if (c != null)
                                {
                                    cache.IsIdentity = true;
                                }

                                //判断当前属性是否具有自定义的属性（数据表字段为主键）
                                c = property.GetCustomAttribute(typeof(PrimaryKeyAttribute));
                                if (c != null)
                                {
                                    cache.IsPrimaryKey = true;
                                }

                                //c = property.GetCustomAttribute(typeof(MultiLineAttribute));
                                //if (c != null)
                                //{
                                //    cache.IsMultiLine = true;
                                //}

                                cache.DataType = property.PropertyType.Name;

                                //将当前的属性，以自定义类型的方式存入实体实例的属性组中
                                instanceEntity.Properties.Add(property.Name.ToLower(), cache);

                                if (cache.IsPrimaryKey)
                                {
                                    instanceEntity.PrimaryKeyProperty = cache;
                                }

                                if (cache.IsIdentity)
                                {
                                    instanceEntity.IdentityProperty = cache;
                                }

                                //if (cache.IsMultiLine)
                                //{
                                //    instanceEntity.MultiLineProperty = cache;
                                //}
                            }
                            #endregion

                        }
                        #endregion

                        //将实例缓存在字典中
                        _InstanceTypeDic.Add(instanceType.FullName, instanceEntity);
                    }
                }
            }

            //将缓存中的实例返回
            return _InstanceTypeDic[instanceType.FullName];
        }

        /// <summary>
        /// 获取属性类型字符串(属性为可为空值类型时,也可以获取到该属性类型)
        /// </summary>
        /// <returns></returns>
        protected string GetPropertyTypeStr(PropertyInfo property)
        {
            Type properyType = property.PropertyType;
            //判断当前属性是否为NULLABLE 的类型
            if (properyType.IsGenericType && properyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // If it is NULLABLE, then get the underlying type. eg if "Nullable<int>" then this will return just "int"
                properyType = property.PropertyType.GetGenericArguments()[0];
            }
            return properyType.Name;
        }

        #endregion

        #region 接口实现

        #region Delete

        /// <summary>
        /// 从数据库删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual bool DeleteFromDB<T>(T entity)
        {
            bool b = false;

            InstanceStoredEntity instanceType = GetInstanceMembers<T>();

            SQLExcutiveCommand sqlCommand = this.GetDeleteCommand(instanceType, entity);

            int i = ExecuteNonQuery(sqlCommand);

            if (i > 0)
            {
                b = true;
            }

            return b;
        }

        /// <summary>
        /// 根据主键从数据库删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        protected virtual bool DeleteFromDBByPrimaryKey<T>(object primaryKey)
        {
            bool b = false;

            InstanceStoredEntity instanceType = GetInstanceMembers<T>();

            SQLExcutiveCommand sqlCommand = this.GetDeleteCommand(instanceType, primaryKey);

            int i = ExecuteNonQuery(sqlCommand);

            if (i > 0)
            {
                b = true;
            }

            return b;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public virtual bool Delete<T>(T entity)
        {
            bool b = false;

            b = DeleteFromDB(entity);

            if (b)
            {
                object primaryKey = GetPrimaryKey(entity);

                b = this.RemoveFromCache<T>(entity);
            }

            return b;
        }

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <typeparam name="T">指定删除类型</typeparam>
        /// <param name="primaryKey">主键</param>
        /// <returns></returns>
        public virtual bool DeleteByPrimaryKey<T>(object primaryKey)
        {
            bool b = false;

            b = DeleteFromDBByPrimaryKey<T>(primaryKey);

            if (b)
            {
                b = this.RemoveFromCacheByPrimaryKey<T>(primaryKey);
            }

            return b;
        }

        #endregion

        #region Update

        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="entity">要更新的实体</param>
        /// <returns>更新状态</returns>
        public virtual bool UpdateDB<T>(T entity)
        {
            bool b = false;

            InstanceStoredEntity instanceType = GetInstanceMembers<T>();

            SQLExcutiveCommand sqlCommand = this.GetUpdateCommand(instanceType, entity);

            int i = ExecuteNonQuery(sqlCommand);

            if (i > 0)
            {
                b = true;
            }

            return b;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public virtual bool Update<T>(T entity)
        {
            bool b = false;
            b = UpdateDB(entity);

            if (b)
            {
                object primaryKey = GetPrimaryKey(entity);

                UpdateCache<T>(primaryKey);

            }

            return b;
        }

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey"></param>
        public virtual void UpdateCache<T>(object primaryKey)
        {
            //更新成功
            //如果开启缓存，从数据库取最新的更新缓存
            if (EnableCache)
            {
                T newEntity = QueryEntireFromDB<T>(primaryKey);
                //Modify a bug by Justin 2013.5.8
                //last version SetToCache(entity);
                SetToCache(newEntity);
            }
        }

        /// <summary>
        /// 更新缓存 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey"></param>
        /// <param name="action"></param>
        public virtual void UpdateCache<T>(object primaryKey, Action<T> action)
        {
            //更新成功
            //如果开启缓存，从数据库取最新的更新缓存
            if (EnableCache)
            {
                T newEntity = QueryEntireFromDB<T>(primaryKey);
                if (action != null)
                {
                    action(newEntity);
                }
                //Modify a bug by Justin 2013.5.8
                //last version SetToCache(entity);
                SetToCache(newEntity);
            }
        }

        #endregion

        #region Insert

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool AsyncInsertToDB(ref AsyncEntity entity)
        {
            bool b = false;

            InstanceStoredEntity instanceType = GetInstanceMembersByObj(entity);

            SQLExcutiveCommand sqlCommand = this.GetInsertCommand(instanceType, entity);

            if (instanceType.IdentityProperty != null)
            {
                object oid = ExecuteScalar(sqlCommand);

                instanceType.IdentityProperty.CurrentProperty.SetValue(entity, oid);
            }
            else
            {
                ExecuteNonQuery(sqlCommand);
            }


            b = true;

            return b;
        }


        #region 导入职位相关

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool IdentityInsert<T>(ref T entity)
        {
            bool b = false;

            InstanceStoredEntity instanceType = GetInstanceMembers<T>();

            SQLExcutiveCommand sqlCommand = this.GetIdentityInsertCommand(instanceType, entity);

            ExecuteNonQuery(sqlCommand);

            b = true;

            return b;
        }

        #endregion

        /// <summary>
        /// 新增一行记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public virtual bool InsertToDB<T>(ref T entity)
        {
            bool b = false;

            InstanceStoredEntity instanceType = GetInstanceMembers<T>();

            SQLExcutiveCommand sqlCommand = this.GetInsertCommand(instanceType, entity);

            if (instanceType.IdentityProperty != null)
            {
                object oid = ExecuteScalar(sqlCommand);

                instanceType.IdentityProperty.CurrentProperty.SetValue(entity, oid);
            }
            else
            {
                ExecuteNonQuery(sqlCommand);
            }

            b = true;

            return b;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public virtual bool Insert<T>(ref T entity)
        {

            bool b = false;

            //插入数据库
            b = InsertToDB(ref entity);


            if (EnableCache)
            {
                SetToCache(entity);
            }

            return b;
        }

        #endregion

        #region Query

        #region SingleEntity

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="primaryKey">主键</param>
        /// <returns></returns>
        public virtual T Query<T>(object primaryKey)
        {

            return Query<T>(primaryKey, null);

        }

        /// <summary>
        /// 从数据库中抽取对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="primaryKey">主键</param>
        /// <returns></returns>
        public virtual T QueryEntireFromDB<T>(object primaryKey)
        {
            T entity = default(T);

            InstanceStoredEntity instanceType = GetInstanceMembers<T>();

            SQLExcutiveCommand executiveCommand = this.GetQueryCommand<T>(instanceType, primaryKey, null);

            DataTable table = GetDataTableFromInstance(executiveCommand);

            if (table.Rows.Count == 0)
            {
                return default(T);
            }

            DataRow dr = table.Rows[0];

            entity = MappingEntity<T>(dr, instanceType);

            return entity;
        }

        /// <summary>
        /// 根据主键获取单个对象
        /// </summary>
        /// <param name="entity">仅赋值主键的实体</param>
        /// <param name="queriedColumns">需要赋值的列名</param>
        /// <returns></returns>
        public T Query<T>(object primaryKey, List<string> queriedColumns)
        {

            InstanceStoredEntity instanceType = GetInstanceMembers<T>();

            T entity = default(T);

            //开启缓存，则缓存所有字段
            if (EnableCache)
            {
                ////test
                //HunterInfo hi = new HunterInfo();
                //hi.Name = "ZhuangFengwei";
                //hi.HunterUserID = 100;
                //SetToCache<HunterInfo>(hi);
                ////test

                //从缓存读取
                entity = GetFromCache<T>(primaryKey, instanceType);

                if (entity == null)
                {
                    entity = QueryEntireFromDB<T>(primaryKey);

                    if (entity != null)
                    {
                        SetToCache<T>(entity);
                    }
                }
            }
            else
            {
                SQLExcutiveCommand executiveCommand = this.GetQueryCommand<T>(instanceType, primaryKey, queriedColumns);

                DataTable table = GetDataTableFromInstance(executiveCommand);

                if (table.Rows.Count == 0)
                {
                    return default(T);
                }

                DataRow dr = table.Rows[0];

                entity = MappingEntity<T>(dr, instanceType);
            }

            return entity;


        }

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public T GetCache<T>(object primaryKey)
        {
            InstanceStoredEntity instanceType = GetInstanceMembers<T>();
            T entity = default(T);

            //开启缓存，则缓存所有字段
            if (EnableCache)
            {
                entity = GetFromCache<T>(primaryKey, instanceType);
            }

            return entity;
        }

        /// <summary>
        /// 保存缓存信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void SetCache<T>(T entity)
        {
            SetToCache<T>(entity);
        }

        /// <summary>
        /// 保存缓存信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey"></param>
        /// <param name="entity"></param>
        public void SetCache<T>(object primaryKey, T entity)
        {
            SetToCache<T>(primaryKey, entity);
        }

        #endregion


        #region List

        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="paramDic">参数词典</param>
        /// <returns></returns>
        public virtual List<T> Query<T>(string where, Dictionary<string, object> paramDic)
        {
            return Query<T>(where, paramDic, null);
        }


        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="paramDic">参数词典</param>
        /// <param name="queriedColumns">需要的数据列（不需要请传null查询全部列）</param>
        /// <returns></returns>
        public virtual List<T> Query<T>(string where, Dictionary<string, object> paramDic, List<string> queriedColumns)
        {
            return Query<T>(where, string.Empty, paramDic, queriedColumns);
        }

        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="orderBy">排序条件</param>
        /// <param name="paramDic">参数词典</param>
        /// <param name="queriedColumns">需要的数据列（不需要请传null查询全部列）</param>
        /// <returns></returns>
        public virtual List<T> Query<T>(string where, string orderBy, Dictionary<string, object> paramDic, List<string> queriedColumns)
        {

            List<T> entityList = new List<T>();

            InstanceStoredEntity instanceType = GetInstanceMembers<T>();

            string sqlTitle = GetQueryCommandTitle(instanceType, queriedColumns);

            string sqlText = string.Empty;

            if (where == null || where == string.Empty)
            {
                sqlText = sqlTitle;
            }
            else
            {
                sqlText = sqlTitle + " where " + where;
            }

            if (orderBy != null && orderBy != string.Empty)
            {
                sqlText += " order by " + orderBy;
            }

            DataTable table = QueryList(sqlText, paramDic);

            if (table != null && table.Rows.Count > 0)
            {


                foreach (DataRow dr in table.Rows)
                {
                    T newEntity = MappingEntity<T>(dr, instanceType);

                    entityList.Add(newEntity);
                }
            }

            return entityList;
        }

        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="dbKey">数据库Key</param>
        /// <param name="sqlText">查询语句</param>
        /// <param name="paramDic">参数词典</param>
        public virtual List<T> Query<T>(string dbKey, string sqlText, Dictionary<string, object> paramDic)
        {
            List<T> entityList = new List<T>();

            InstanceStoredEntity instanceType = GetInstanceMembers<T>();

            DataTable table = QueryList(sqlText, paramDic);

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow dr in table.Rows)
                {
                    T newEntity = MappingEntity<T>(dr, instanceType);

                    entityList.Add(newEntity);
                }
            }

            return entityList;
        }


        /// <summary>
        ///  查询数据列表分页
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="dbKey">数据库Key</param>
        /// <param name="sqlText">查询语句</param>
        /// <param name="paramDic">参数字典</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页数</param>
        /// <param name="itemCount">总数</param>
        /// <param name="orderbyPart">排序字段</param>
        /// <returns></returns>
        public virtual List<T> Query<T>(string dbKey, string sqlText, IDictionary<string, object> paramDic, int pageIndex, int pageSize, out int itemCount, string orderbyPart)
        {

            List<T> entityList = new List<T>();

            InstanceStoredEntity instanceType = GetInstanceMembers<T>();

            DataTable table = QueryList(dbKey, sqlText, paramDic, pageIndex, pageSize, out itemCount, orderbyPart);

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow dr in table.Rows)
                {
                    T newEntity = MappingEntity<T>(dr, instanceType);

                    entityList.Add(newEntity);
                }
            }

            return entityList;
        }

        /// <summary>
        ///  查询数据列表分页
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="dbKey">数据库Key</param>
        /// <param name="sqlText">查询语句</param>
        /// <param name="paramDic">参数字典</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页数</param>
        /// <param name="itemCount">总数</param>
        /// <param name="orderbyPart">排序字段</param>
        /// <returns></returns>
        public virtual List<T> Query<T>(string dbKey, string sqlText, SqlParameter[] paramList, int pageIndex, int pageSize, out int itemCount, string orderbyPart)
        {

            List<T> entityList = new List<T>();

            InstanceStoredEntity instanceType = GetInstanceMembers<T>();

            DataTable table = QueryList(dbKey, sqlText, paramList, pageIndex, pageSize, out itemCount, orderbyPart);

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow dr in table.Rows)
                {
                    T newEntity = MappingEntity<T>(dr, instanceType);

                    entityList.Add(newEntity);
                }
            }

            return entityList;
        }

        /// <summary>
        /// 从缓存中查询信息
        /// </summary>
        /// <typeparam name="T">信息的类型</typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual T QueryCache<T>(Object primaryKey)
        {
            InstanceStoredEntity instanceType = GetInstanceMembers<T>();
            T entity = default(T);
            //开启缓存，则缓存所有字段
            if (EnableCache)
            {
                entity = GetFromCache<T>(primaryKey, instanceType);
            }

            return entity;
        }

        /// <summary>
        /// 将对象保存到缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">实体</param>
        /// <returns></returns>
        public virtual bool SaveCache<T>(T entity) where T : class
        {
            bool isRight = false;

            InstanceStoredEntity instanceType = GetInstanceMembers<T>();
            //开启缓存，则缓存所有字段
            if (EnableCache)
            {
                if (entity != null)
                {
                    isRight = true;
                    SetToCache<T>(entity);
                }
            }

            return isRight;
        }


        #endregion

        #endregion

        #region 数据库基本字段映射

        #region 子类可以自己实现

        /// <summary>
        /// 获取实际的数据库访问器
        /// </summary>
        /// <returns></returns>
        public virtual SQLHelper GetExecuteSQLHelper(string dbKey)
        {
            SQLHelper accesser = DBAccessor.GetDataAccess(dbKey);

            return accesser;
        }

        /// <summary>
        /// 获取当前实际数据库访问节点
        /// </summary>
        /// <returns></returns>
        public virtual SQLHelper GetCurrentSqlHelper()
        {
            return GetExecuteSQLHelper(DBKey);
        }

        #endregion

        /// <summary>
        /// 执行SQL更新
        /// </summary>
        /// <param name="executiveCommand">SQL执行实体</param>
        /// <returns></returns>
        private int ExecuteNonQuery(SQLExcutiveCommand executiveCommand)
        {
            SQLHelper helper = GetCurrentSqlHelper();

            SqlParameter[] dbParams = DBAccessor.Transfrom(executiveCommand.ParamDic);

            return helper.ExecuteNonQuery(executiveCommand.SQLText, CommandType.Text, dbParams);
        }


        /// <summary>
        /// 执行新增
        /// </summary>
        /// <param name="executiveCommand"></param>
        /// <returns></returns>
        private object ExecuteScalar(SQLExcutiveCommand executiveCommand)
        {
            SQLHelper helper = GetCurrentSqlHelper();

            SqlParameter[] dbParams = DBAccessor.Transfrom(executiveCommand.ParamDic);

            return helper.ExecuteScalar(executiveCommand.SQLText, CommandType.Text, dbParams);
        }

        /// <summary>
        /// 获取执行结果DataTable
        /// </summary>
        /// <param name="executiveCommand">执行命令</param>
        /// <returns></returns>
        private DataTable GetDataTableFromInstance(SQLExcutiveCommand executiveCommand)
        {
            return QueryList(executiveCommand.SQLText, executiveCommand.ParamDic);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="sqlText">SQL语句</param>
        /// <param name="paramDic">参数词典</param>
        /// <returns></returns>
        private DataTable QueryList(string sqlText, Dictionary<string, object> paramDic)
        {
            SQLHelper helper = GetCurrentSqlHelper();

            SqlParameter[] dbParams = DBAccessor.Transfrom(paramDic);

            return helper.ExecuteDataSet(sqlText, CommandType.Text, dbParams).Tables[0];
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="dbKey">数据库访问节点</param>
        /// <param name="sqlText">数据库语句</param>
        /// <param name="paramDic">参数词典</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页显示条目</param>
        /// <param name="itemCount">总记录数</param>
        /// <param name="orderbyPart">排序字段</param>
        /// <returns></returns>
        private DataTable QueryList(string dbKey, string sqlText, IDictionary<string, object> paramDic, int pageIndex, int pageSize, out int itemCount, string orderbyPart)
        {
            SQLHelper helper = GetExecuteSQLHelper(dbKey);

            SqlParameter[] dbParams = DBAccessor.Transfrom(paramDic);

            return helper.ExecuteTableForPage(pageIndex, pageSize, out itemCount, sqlText, dbParams, orderbyPart);
        }

        /// <summary>
        /// 传递原生参数进行分页
        /// </summary>
        /// <param name="dbKey"></param>
        /// <param name="sqlText"></param>
        /// <param name="paramDic"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="itemCount"></param>
        /// <param name="orderbyPart"></param>
        /// <returns></returns>
        private DataTable QueryList(string dbKey, string sqlText, SqlParameter[] paramList, int pageIndex, int pageSize, out int itemCount, string orderbyPart)
        {
            SQLHelper helper = GetExecuteSQLHelper(dbKey);

            return helper.ExecuteTableForPage(pageIndex, pageSize, out itemCount, sqlText, paramList, orderbyPart);
        }


        #endregion

        #endregion
    }
}
