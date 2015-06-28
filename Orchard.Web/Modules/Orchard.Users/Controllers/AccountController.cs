using System;
using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using System.Web.Security;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Mvc.Extensions;
using Orchard.Security;
using Orchard.Users.Services;
using Orchard.ContentManagement;
using Orchard.Users.Models;
using Orchard.UI.Notify;
using Orchard.Users.Events;
using Orchard.Utility.Extensions;
using Orchard.Users.ViewModels;

namespace Orchard.Users.Controllers
{
    [HandleError]
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipService _membershipService;
        private readonly IUserService _userService;
        private readonly IOrchardServices _orchardServices;
        private readonly IUserEventHandler _userEventHandler;

        public AccountController(
            IAuthenticationService authenticationService,
            IMembershipService membershipService,
            IUserService userService,
            IOrchardServices OrchardServices,
            IUserEventHandler userEventHandler)
        {
            _authenticationService = authenticationService;
            _membershipService = membershipService;
            _userService = userService;
            _orchardServices = OrchardServices;
            _userEventHandler = userEventHandler;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        [AlwaysAccessible]
        public ActionResult AccessDenied()
        {
            var returnUrl = Request.QueryString["ReturnUrl"];
            var currentUser = _authenticationService.GetAuthenticatedUser() as UserRecord;

            //登录
            if (currentUser == null)
            {
                Logger.Information("Access denied to anonymous request on {0}", returnUrl);
                return View("Logon","Layout2");
            }

            //TODO: (erikpo) Add a setting for whether or not to log access denieds since these can fill up a database pretty fast from bots on a high traffic site
            //Suggestion: Could instead use the new AccessDenined IUserEventHandler method and let modules decide if they want to log this event?
            Logger.Information("Access denied to user #{0} '{1}' on {2}", currentUser.Id, currentUser.UserName, returnUrl);

            _userEventHandler.AccessDenied(currentUser);

            return View();
        }

        [AlwaysAccessible]
        public ActionResult LogOn(string returnUrl)
        {
            if (_authenticationService.GetAuthenticatedUser() != null)
                return this.RedirectLocal(returnUrl);

            return View("Logon", "Layout2");
        }

        [HttpPost]
        [AlwaysAccessible]
        [ValidateInput(false)]
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings",
            Justification = "Needs to take same parameter type as Controller.Redirect()")]
        public ActionResult LogOn(LogonViewModel model)
        {
            if (ModelState.IsValid)
            {
                _userEventHandler.LoggingIn(model.UserNameOrEmail, model.Password);
                var user = ValidateLogOn(model.UserNameOrEmail, model.Password);
                _authenticationService.SignIn(user, model.RememberMe);
                _userEventHandler.LoggedIn(user);
                return this.RedirectLocal(model.ReturnUrl);
            }

            return View("Logon", "Layout2");
        }

        public ActionResult LogOff(string returnUrl)
        {
            var wasLoggedInUser = _authenticationService.GetAuthenticatedUser();
            _authenticationService.SignOut();
            if (wasLoggedInUser != null)
                _userEventHandler.LoggedOut(wasLoggedInUser);
            return this.RedirectLocal(returnUrl);
        }

        int MinPasswordLength
        {
            get
            {
                return 6;
            }
        }

        [AlwaysAccessible]
        public ActionResult Register()
        {
            // ensure users can register
            ViewData["PasswordLength"] = MinPasswordLength;

            return View("Register");
        }

        [HttpPost]
        [AlwaysAccessible]
        [ValidateInput(false)]
        public ActionResult Register(string userName, string email, string password, string confirmPassword, string returnUrl = null)
        {
            ViewData["PasswordLength"] = MinPasswordLength;

            if (ValidateRegistration(userName, email, password, confirmPassword))
            {
                // Attempt to register the user
                // No need to report this to IUserEventHandler because _membershipService does that for us
                var user = _membershipService.CreateUser(new CreateUserParams(userName, password, email, false)) as UserRecord;

                if (user != null)
                {
                    if (user.EmailStatus == UserStatus.Pending)
                    {

                        //_userService.SendChallengeEmail(user.As<UserPart>(), nonce => Url.MakeAbsolute(Url.Action("ChallengeEmail", "Account", new { Area = "Orchard.Users", nonce = nonce }), siteUrl));

                        _userEventHandler.SentChallengeEmail(user);
                        return RedirectToAction("ChallengeEmailSent");
                    }

                    if (user.RegistrationStatus == UserStatus.Pending)
                    {
                        return RedirectToAction("RegistrationPending");
                    }

                    _authenticationService.SignIn(user, false /* createPersistentCookie */);
                    return this.RedirectLocal(returnUrl);
                }

                ModelState.AddModelError("_FORM", ErrorCodeToString(/*createStatus*/MembershipCreateStatus.ProviderError));
            }

            // If we got this far, something failed, redisplay form
            return View("Register");
        }

        [AlwaysAccessible]
        public ActionResult RequestLostPassword()
        {
            return View();
        }

        [HttpPost]
        [AlwaysAccessible]
        public ActionResult RequestLostPassword(string username)
        {

            if (String.IsNullOrWhiteSpace(username))
            {
                ModelState.AddModelError("userNameOrEmail", "Invalid username or E-mail.");
                return View();
            }


            _orchardServices.Notifier.Information("Check your e-mail for the confirmation link.");
            return RedirectToAction("LogOn");
        }

        [Authorize]
        [AlwaysAccessible]
        public ActionResult ChangePassword()
        {
            ViewData["PasswordLength"] = MinPasswordLength;

            return View();
        }

        [Authorize]
        [HttpPost]
        [AlwaysAccessible]
        [ValidateInput(false)]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Exceptions result in password not being changed.")]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            ViewData["PasswordLength"] = MinPasswordLength;

            if (!ValidateChangePassword(currentPassword, newPassword, confirmPassword))
            {
                return View();
            }

            try
            {
                var validated = _membershipService.ValidateUser(User.Identity.Name, currentPassword);

                if (validated != null)
                {
                    _membershipService.SetPassword(validated, newPassword);
                    _userEventHandler.ChangedPassword(validated);
                    return RedirectToAction("ChangePasswordSuccess");
                }

                ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
                return ChangePassword();
            }
            catch
            {
                ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
                return ChangePassword();
            }
        }

        public ActionResult LostPassword(string nonce)
        {
            if (_userService.ValidateLostPassword(nonce) == null)
            {
                return RedirectToAction("LogOn");
            }

            ViewData["PasswordLength"] = MinPasswordLength;

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult LostPassword(string nonce, string newPassword, string confirmPassword)
        {
            IUser user;
            if ((user = _userService.ValidateLostPassword(nonce)) == null)
            {
                return Redirect("~/");
            }

            ViewData["PasswordLength"] = MinPasswordLength;

            if (newPassword == null || newPassword.Length < MinPasswordLength)
            {
                ModelState.AddModelError("newPassword", string.Format("You must specify a new password of {0} or more characters.", MinPasswordLength));
            }

            if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", string.Format("The new password and confirmation password do not match."));
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            _membershipService.SetPassword(user, newPassword);

            _userEventHandler.ChangedPassword(user);

            return RedirectToAction("ChangePasswordSuccess");
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public ActionResult RegistrationPending()
        {
            return View();
        }

        public ActionResult ChallengeEmailSent()
        {
            return View();
        }

        public ActionResult ChallengeEmailSuccess()
        {
            return View();
        }

        public ActionResult ChallengeEmailFail()
        {
            return View();
        }

        public ActionResult ChallengeEmail(string nonce)
        {
            var user = _userService.ValidateChallenge(nonce);

            if (user != null)
            {
                _userEventHandler.ConfirmedEmail(user);

                return RedirectToAction("ChallengeEmailSuccess");
            }

            return RedirectToAction("ChallengeEmailFail");
        }

        #region Validation Methods
        private bool ValidateChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (String.IsNullOrEmpty(currentPassword))
            {
                ModelState.AddModelError("currentPassword", "You must specify a current password.");
            }
            if (newPassword == null || newPassword.Length < MinPasswordLength)
            {
                ModelState.AddModelError("newPassword", string.Format("You must specify a new password of {0} or more characters.", MinPasswordLength));
            }

            if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", string.Format("The new password and confirmation password do not match."));
            }

            return ModelState.IsValid;
        }


        private IUser ValidateLogOn(string userNameOrEmail, string password)
        {
            bool validate = true;

            if (String.IsNullOrEmpty(userNameOrEmail))
            {
                ModelState.AddModelError("userNameOrEmail", "You must specify a username or e-mail.");
                validate = false;
            }
            if (String.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "You must specify a password.");
                validate = false;
            }

            if (!validate)
                return null;

            var user = _membershipService.ValidateUser(userNameOrEmail, password);
            if (user == null)
            {
                _userEventHandler.LogInFailed(userNameOrEmail, password);
                ModelState.AddModelError("_FORM", "The username or e-mail or password provided is incorrect.");
            }

            return user;
        }

        private bool ValidateRegistration(string userName, string email, string password, string confirmPassword)
        {
            bool validate = true;

            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", "You must specify a username.");
                validate = false;
            }
            //else
            //{
            //    if (userName.Length >= UserPart.MaxUserNameLength)
            //    {
            //        ModelState.AddModelError("username", T("The username you provided is too long."));
            //        validate = false;
            //    }
            //}

            if (String.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("email", "You must specify an email address.");
                validate = false;
            }
            //else if (email.Length >= UserPart.MaxEmailLength)
            //{
            //    ModelState.AddModelError("email", T("The email address you provided is too long."));
            //    validate = false;
            //}
            //else if (!Regex.IsMatch(email, UserPart.EmailPattern, RegexOptions.IgnoreCase))
            //{
            //    // http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx    
            //    ModelState.AddModelError("email", "You must specify a valid email address.");
            //    validate = false;
            //}

            if (!validate)
                return false;

            if (!_userService.VerifyUserUnicity(userName, email))
            {
                ModelState.AddModelError("userExists", "User with that username and/or email already exists.");
            }
            if (password == null || password.Length < MinPasswordLength)
            {
                ModelState.AddModelError("password", string.Format("You must specify a password of {0} or more characters.", MinPasswordLength));
            }
            if (!String.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
            }
            return ModelState.IsValid;
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return
                        "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return
                        "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return
                        "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion
    }
}