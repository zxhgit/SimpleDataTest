using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Zx.Web.ApiContainer.model;

namespace Zx.Web.ApiContainer.framework
{
    public class AuthService
    {
        private static readonly List<Tuple<string, string>> Users = new List<Tuple<string, string>>();
        private static readonly Dictionary<string, List<string>> Claims = new Dictionary<string, List<string>>();

        public static readonly bool IsAuth = false;

        static AuthService()
        {
            var isAuth = ConfigurationManager.AppSettings["IsAuth"];
            if (isAuth.ToLower() != "true") return;
            IsAuth = true;

            Users.Add(new Tuple<string, string>("demo", "demo"));
            Claims.Add("demo", new List<string> { "demo", "admin" });

            Users.Add(new Tuple<string, string>("nonadmin", "nonadmin"));
            Claims.Add("nonadmin", new List<string> { "demo", });
        }

        public static IUserIdentity ValidateUser(string userName, string password)
        {
            var user = Users.FirstOrDefault(x => x.Item1 == userName && x.Item2 == password);
            if (user == null)
                return null;
            var claims = Claims[user.Item1];
            return new AuthUserIdentity { UserName = user.Item1, Claims = claims };
        }
    }
}