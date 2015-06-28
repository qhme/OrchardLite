using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Orchard.Environment.Configuration;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Services;

namespace Orchard.Security.Providers
{
    public class FormsAuthenticationService : IAuthenticationService
    {
        private readonly ShellSettings _settings;
        private readonly IClock _clock;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IUser _signedInUser;
        private bool _isAuthenticated;
        private readonly IMembershipService _membershipService;

        public FormsAuthenticationService(ShellSettings settings, IClock clock, IHttpContextAccessor httpContextAccessor,
            IMembershipService membershipService)
        {
            _settings = settings;
            _clock = clock;
            _httpContextAccessor = httpContextAccessor;
            _membershipService = membershipService;
            Logger = NullLogger.Instance;
            ExpirationTimeSpan = TimeSpan.FromDays(30);
        }

        public ILogger Logger { get; set; }

        public TimeSpan ExpirationTimeSpan { get; set; }

        public void SignIn(IUser user, bool createPersistentCookie)
        {
            var now = _clock.UtcNow.ToLocalTime();

            // the cookie user data is {userId};{tenant}
            var userData = Convert.ToString(user.Id);

            var ticket = new FormsAuthenticationTicket(
                1 /*version*/,
                user.UserName,
                now,
                now.Add(ExpirationTimeSpan),
                createPersistentCookie,
                userData,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL,
                Path = FormsAuthentication.FormsCookiePath
            };

            var httpContext = _httpContextAccessor.Current();

            cookie.Path = GetCookiePath(httpContext);

            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            if (createPersistentCookie)
            {
                cookie.Expires = ticket.Expiration;
            }

            httpContext.Response.Cookies.Add(cookie);

            _isAuthenticated = true;
            _signedInUser = user;
        }

        public void SignOut()
        {
            _signedInUser = null;
            _isAuthenticated = false;
            FormsAuthentication.SignOut();

            // overwritting the authentication cookie for the given tenant
            var httpContext = _httpContextAccessor.Current();
            var rFormsCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "")
            {
                Expires = DateTime.Now.AddYears(-1),
            };

            rFormsCookie.Path = GetCookiePath(httpContext);
            httpContext.Response.Cookies.Add(rFormsCookie);
        }

        public void SetAuthenticatedUserForRequest(IUser user)
        {
            _signedInUser = user;
            _isAuthenticated = true;
        }

        public IUser GetAuthenticatedUser()
        {
            if (_signedInUser != null || _isAuthenticated)
                return _signedInUser;

            var httpContext = _httpContextAccessor.Current();
            if (httpContext == null || !httpContext.Request.IsAuthenticated || !(httpContext.User.Identity is FormsIdentity))
            {
                return null;
            }

            var formsIdentity = (FormsIdentity)httpContext.User.Identity;
            var userDataId = formsIdentity.Ticket.UserData ?? "";

            // the cookie user data is {userId};{tenant}
            //var userDataSegments = userData.Split(';');

            //if (userDataSegments.Length < 2)
            //{
            //    return null;
            //}

            int userId;
            if (!int.TryParse(userDataId, out userId))
            {
                Logger.Error("User id not a parsable integer");
                return null;
            }

            _isAuthenticated = true;

            return _signedInUser = _membershipService.GetUser(userId);
        }

        private string GetCookiePath(HttpContextBase httpContext)
        {
            var cookiePath = httpContext.Request.ApplicationPath;
            if (cookiePath != null && cookiePath.Length > 1)
            {
                cookiePath += '/';
            }


            return cookiePath;
        }
    }

}
