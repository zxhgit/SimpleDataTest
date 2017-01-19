using System;
using System.Collections.Generic;

namespace Zx.ApiAdmin.Entity.Admin
{
    public class ApiContainerMethodInfo
    {
        public string Name { get; set; }

        public bool IsStatic { get; set; }

        public string ReturnTypeName { get; set; }

        public string ReturnTypeFullName { get; set; }

        public List<Tuple<string, string, string>> ParmTypes { get; set; }
    }
}
