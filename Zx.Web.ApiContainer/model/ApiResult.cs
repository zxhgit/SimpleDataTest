using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zx.Web.ApiContainer.model.Enum;

namespace Zx.Web.ApiContainer.model
{
    public class ApiResult
    {
        public ServiceErrorCodeEnum Code { get; set; }
        public object Data { get; set; }
    }
}