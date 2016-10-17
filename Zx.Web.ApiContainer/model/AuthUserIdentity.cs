using Nancy.Security;
using System.Collections.Generic;

namespace Zx.Web.ApiContainer.model
{
    public class AuthUserIdentity : IUserIdentity
    {
        public string UserName { get; set; }
        public IEnumerable<string> Claims { get; set; }
    }
}