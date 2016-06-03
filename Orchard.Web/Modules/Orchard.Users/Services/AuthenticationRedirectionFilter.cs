using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;
using Orchard.Mvc.Filters;
using System.Linq;

namespace Orchard.Users.Services
{
    public class AuthenticationRedirectionFilter : FilterProvider, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;

                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                {
                    { "controller", "Account" },
                    { "action", "AccessDenied" },
                    { "area", "Orchard.Users"},
                    { "ReturnUrl", filterContext.HttpContext.Request.RawUrl }
                });
            }
        }
    }
}