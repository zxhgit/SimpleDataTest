using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zx.Web.ApiContainer.framework
{
    public interface IExceptionLogService
    {
        void LogError(Exception ex);
    }
}
