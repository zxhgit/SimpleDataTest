using Nancy;
using Nancy.Authentication.Token;
using Zx.Web.ApiContainer.framework;

namespace Zx.Web.ApiContainer
{
    public class AuthModule : NancyModule
    {
        public AuthModule(ITokenizer tokenizer)
            : base("/auth")
        {
            Post["/"] = x =>
            {
                var userName = (string)Request.Form.UserName;
                var password = (string)Request.Form.Password;
                var userIdentity = AuthService.ValidateUser(userName, password);
                if (userIdentity == null)
                    return HttpStatusCode.Unauthorized;
                var token = tokenizer.Tokenize(userIdentity, Context);
                return new { Token = token };
            };
        }
    }
}