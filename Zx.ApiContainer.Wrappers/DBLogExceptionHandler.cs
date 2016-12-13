using System;
using System.Collections.Generic;
using System.Linq;
using Zx.ApiContainer.Utilities.Exceptions;

namespace Zx.ApiContainer.Wrappers
{
    /// <summary>
    /// 
    /// </summary>
    public class DBLogExceptionHandler : IExceptionHandler
    {
        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="exception"></param>
        public void Process(Exception exception)
        {
            try
            {
                ILogService service = ServiceFactory.GetService<ILogService>() as ILogService;

                service.SendExceptionLog(exception);
            }
            catch (Exception ex)
            {
                IExceptionHandler defaultHandler = ExceptionHandlerFactory.GetDefaultExceptionProcessor();

                defaultHandler.Process(ex);
            }

        }

    }
}
