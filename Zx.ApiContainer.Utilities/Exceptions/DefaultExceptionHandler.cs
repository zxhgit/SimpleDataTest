using System;
using System.IO;
using System.Text;

namespace Zx.ApiContainer.Utilities.Exceptions
{
    /// <summary>
    /// 默认异常处理器
    /// </summary>
    public class DefaultExceptionHandler : IExceptionHandler
    {
        #region Static Fileds

        /// <summary>
        /// 写入锁定
        /// </summary>
        private static object _lockPad = new object();

        /// <summary>
        /// 写入种子
        /// </summary>
        private static long _Seed = 0;

        #endregion

        #region Const Members

        /// <summary>
        /// 异常本地文件名左边部分
        /// </summary>
        const string EXCEPTION_FILE_LEFT_NANME = "DefaultExceptionLog_";

        /// <summary>
        /// 异常本地文件名后缀
        /// </summary>
        const string EXCEPTION_FILE_EXTEND_NAME = ".log";

        /// <summary>
        /// 异常本地文件名日期扩展部分
        /// </summary>
        const string EXCEPTION_FILE_DATAFORMAT_NAME = "yyyy-MM-dd";



        #endregion

        #region IExceptionProcessor 成员

        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="exception">异常信息</param>
        /// <param name="exceptionTagName">异常标签名称</param>
        /// <param name="primaryKey">主键（可空）</param>
        /// <param name="processorID">业务模块处理器ID</param>
        /// <param name="customerExceptionTitle">自定义异常Title</param>
        public void Process(Exception exception, int? processorID, string exceptionTagName, object primaryKey, string customerExceptionTitle)
        {
            Process(exception);
        }

        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="exception">异常信息</param>
        public virtual void Process(Exception exception)
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + EXCEPTION_FILE_LEFT_NANME + DateTime.Now.ToString(EXCEPTION_FILE_DATAFORMAT_NAME) + EXCEPTION_FILE_EXTEND_NAME;

            StringBuilder builder = new StringBuilder();

            lock (_lockPad)
            {
                _Seed++;
                builder.Append(string.Format("{0}==================={1}===================\r\n", _Seed, DateTime.Now.ToString()));
            }

            BuildExceptionString(builder, exception);

            lock (_lockPad)
            {
                using (StreamWriter writer = new StreamWriter(file, true))
                {
                    writer.WriteLine(builder.ToString());
                }
            }
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// 构建异常信息
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="ex"></param>
        private void BuildExceptionString(StringBuilder builder, Exception ex)
        {
            //if (ex.GetType() == typeof(MigrationException))
            //{
            //    var exM = (MigrationException)ex;

            //    builder.Append("ExceptionKey:\r\n");
            //    builder.Append(exM.ExceptionKey);

            //    builder.Append("ExceptionCategory:\r\n");
            //    builder.Append(exM.ExceptionCategory);

            //    builder.Append("CurrentContext:\r\n");
            //    builder.Append(exM.CurrentContext.ToString());
            //}

            builder.Append("Exception message:");
            builder.Append(ex.Message);
            builder.Append(ex.GetType().ToString());
            builder.Append("\r\n");
            builder.Append("Stack trace:\r\n");
            builder.Append(ex.StackTrace);


            if (ex.InnerException != null)
            {
                builder.Append("\r\n#############################################################\r\n");
                BuildExceptionString(builder, ex.InnerException);
            }
            else
            {
                builder.Append("\r\n==========================================================\r\n\r\n");
            }
        }

        #endregion
    }
}
