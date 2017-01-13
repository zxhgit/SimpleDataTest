using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Zx.ApiContainer.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public class EntityHelper
    {
        /// <summary>
        /// 服务端实现类存储词典 key:类型FullName value：类构造函数
        /// </summary>
        static Dictionary<string, ConstructorInfo> _EntityConstructoerDic = new Dictionary<string, ConstructorInfo>();


        /// <summary>
        /// 单点锁定
        /// </summary>
        static object _LockPad = new object();

        /// <summary>
        /// 构造一个指定类型的实例
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <returns></returns>
        public static object GetEntity(Type entityType)
        {
            string key = entityType.FullName;

            ConstructorInfo constructor = null;

            if (!_EntityConstructoerDic.ContainsKey(key))
            {
                lock (_LockPad)
                {
                    if (!_EntityConstructoerDic.ContainsKey(key))
                    {
                        //获取实体类型的默认构造器
                        constructor = ReflectHelper.GetDefaultConstructorInfo(entityType);

                        //将构造器加入实体构造器缓存字典
                        _EntityConstructoerDic.Add(key, constructor);
                    }
                }
            }

            if (constructor == null)
            {
                constructor = _EntityConstructoerDic[key];
            }

            return constructor.Invoke(null);
        }

        /// <summary>
        /// 复制实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pTargetObjSrc"></param>
        /// <param name="pTargetObjDest"></param>
        public static void EntityToEntity<T>(T pTargetObjSrc, T pTargetObjDest)
        {
            try
            {
                foreach (var mItem in typeof(T).GetProperties())
                {
                    if (mItem.CanRead && mItem.CanWrite)
                    {
                        mItem.SetValue(pTargetObjDest, mItem.GetValue(pTargetObjSrc, new object[] { }), null);
                    }

                }
            }
            catch (NullReferenceException NullEx)
            {
                throw NullEx;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        /// <summary>
        /// Table转成List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<T> Datatable2List<T>(DataTable source) where T : class
        {
            List<T> results = new List<T>();
            Type tType = typeof(T);
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.GetProperty;
            foreach (DataRow row in source.Rows)
            {
                T target = Activator.CreateInstance<T>();
                foreach (DataColumn column in source.Columns)
                {
                    string columnName = column.ColumnName;
                    //get the value from source

                    object value = row[columnName];

                    if (value == DBNull.Value)
                    {
                        value = null;

                        continue;
                    }

                    PropertyInfo p = tType.GetProperty(columnName, flags);
                    if (p != null)
                    {
                        MethodInfo set = p.GetSetMethod();
                        if (set != null)
                        {
                            set.Invoke(target, new object[] { value });
                        }
                    }
                }
                results.Add(target);
            }
            return results;
        }
    }
}
