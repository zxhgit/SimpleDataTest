using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zx.ApiContainer.Utilities.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="exception">异常</param>
        void Process(Exception exception);

    }
}
