using System.Collections.Generic;
using Zx.ApiContainer.Entity;

namespace Zx.ApiContainer.Services.DataAdapters
{
    public interface IBasicDataAdapter
    {
        /// <summary>
        /// 是否开启数据缓存
        /// </summary>
        bool EnableCache
        {
            get;
            set;
        }

        /// <summary>
        /// 是否异步
        /// </summary>
        bool Asynchronous
        {
            get;
            set;
        }

        /// <summary>
        /// 是否禁用空值更新
        /// </summary>
        bool DisableNullUpdate
        {
            get;
            set;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        bool Insert<T>(ref T entity);

        /// <summary>
        /// 插入数据，不更新缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool InsertToDB<T>(ref T entity);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        bool Update<T>(T entity);

        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="entity">要更新的实体</param>
        /// <returns>更新状态</returns>
        bool UpdateDB<T>(T entity);

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey"></param>
        void UpdateCache<T>(object primaryKey);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        bool Delete<T>(T entity);

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        bool DeleteByPrimaryKey<T>(object primaryKey);

        /// <summary>
        /// 根据主键获取单个对象（获取全部对象）
        /// </summary>
        /// <param name="entity">仅赋值主键的实体</param>
        /// <returns></returns>
        T Query<T>(object primaryKey);

        /// <summary>
        /// 从数据库中抽取对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="primaryKey">主键</param>
        /// <returns></returns>
        T QueryEntireFromDB<T>(object primaryKey);

        /// <summary>
        /// 根据主键获取单个对象
        /// </summary>
        /// <param name="entity">仅赋值主键的实体</param>
        /// <param name="queriedColumns">需要赋值的列名</param>
        /// <returns></returns>
        T Query<T>(object primaryKey, List<string> queriedColumns);

        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="paramDic">参数词典</param>
        /// <returns></returns>
        List<T> Query<T>(string where, Dictionary<string, object> paramDic);

        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="paramDic">参数词典</param>
        /// <param name="queriedColumns">需要的数据列（不需要请传null查询全部列）</param>
        /// <returns></returns>
        List<T> Query<T>(string where, Dictionary<string, object> paramDic, List<string> queriedColumns);

        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="orderBy">排序条件</param>
        /// <param name="paramDic">参数词典</param>
        /// <param name="queriedColumns">需要的数据列（不需要请传null查询全部列）</param>
        /// <returns></returns>
        List<T> Query<T>(string where, string orderBy, Dictionary<string, object> paramDic, List<string> queriedColumns);

        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="dbKey">数据库Key</param>
        /// <param name="sqlText">查询语句</param>
        /// <param name="paramDic">参数词典</param>
        /// <returns></returns>
        List<T> Query<T>(string dbKey, string sqlText, Dictionary<string, object> paramDic);

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
        List<T> Query<T>(string dbKey, string sqlText, IDictionary<string, object> paramDic, int pageIndex, int pageSize, out int itemCount, string orderbyPart);


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool AsyncInsertToDB(ref AsyncEntity entity);


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool IdentityInsert<T>(ref T entity);


    }
}
