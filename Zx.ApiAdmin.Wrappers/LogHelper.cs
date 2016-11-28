using System;
using Zx.ApiAdmin.Utilities.Exceptions;

namespace Zx.ApiAdmin.Wrappers
{
    /// <summary>
    /// 
    /// </summary>
    public class LogHelper
    {
        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="ex"></param>
        public static void LogException(Exception ex)
        {
            if (ConfigrationHelper.GlobalEnableAsync)
            {
                string handler = "Zx.ApiAdmin.Wrappers.DBLogExceptionHandler,Zx.ApiAdmin.Wrappers";

                IExceptionHandler processor = ExceptionHandlerFactory.GetCurrentExceptionHandler(handler);

                if (processor != null)
                {
                    try
                    {
                        processor.Process(ex);
                    }
                    catch (Exception exLog)
                    {
                        LogTextException(ex);

                        LogTextException(exLog);

                    }

                }
            }
            else
            {
                LogHelper.LogTextException(ex);
            }

        }

        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="ex"></param>
        public static void LogTextException(Exception ex)
        {

            IExceptionHandler processor = ExceptionHandlerFactory.GetDefaultExceptionProcessor();

            if (processor != null)
            {
                processor.Process(ex);
            }

        }

    }
}
