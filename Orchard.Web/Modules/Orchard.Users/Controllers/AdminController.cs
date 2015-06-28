using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Core.Settings.Models;
using Orchard.Data;
using Orchard.Mvc;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Users.Events;
using Orchard.Users.Models;
using Orchard.Users.Services;
using Orchard.Users.ViewModels;

namespace Orchard.Users.Controllers
{
    [ValidateInput(false)]
    public class AdminController : Controller
    {
        private readonly IMembershipService _membershipService;
        private readonly IUserService _userService;
        private readonly IUserEventHandler _userEventHandlers;
        private readonly ISiteService _siteService;
        private readonly IRepository<UserRecord> _userRepository;
        private readonly ISettingService _settingService;

        public AdminController(
            IOrchardServices services,
            IMembershipService membershipService,
            IUserService userService,
             IUserEventHandler userEventHandlers,
            IRepository<UserRecord> userRepository,
            ISettingService settingService,
            ISiteService siteService)
        {
            Services = services;
            _membershipService = membershipService;
            _userService = userService;
            _userEventHandlers = userEventHandlers;
            _siteService = siteService;
            _userRepository = userRepository;
            _settingService = settingService;
        }

        public IOrchardServices Services { get; set; }

        public ActionResult Index(UserIndexOptions options, PagerParameters pagerParameters)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageUsers, "Not authorized to list users"))
                return new HttpUnauthorizedResult();

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            // default options
            if (options == null)
                options = new UserIndexOptions();

            var users = _userRepository.Table;

            switch (options.Filter)
            {
                case UsersFilter.Approved:
                    users = users.Where(u => u.RegistrationStatus == UserStatus.Approved);
                    break;
                case UsersFilter.Pending:
                    users = users.Where(u => u.RegistrationStatus == UserStatus.Pending);
                    break;
                case UsersFilter.EmailPending:
                    users = users.Where(u => u.EmailStatus == UserStatus.Pending);
                    break;
            }

            if (!String.IsNullOrWhiteSpace(options.Search))
            {
                users = users.Where(u => u.UserName.Contains(options.Search) || u.Email.Contains(options.Search));
            }


            switch (options.Order)
            {
                case UsersOrder.Name:
                    users = users.OrderBy(u => u.UserName);
                    break;
                case UsersOrder.Email:
                    users = users.OrderBy(u => u.Email);
                    break;
            }

            pager.Total = users.Count();
            var results = users.Skip((pager.Page - 1) * pager.PageSize).Take(pager.PageSize)
                .ToList();

            var model = new UsersIndexViewModel
            {
                Users = results
                    .Select(x => new UserEntry { User = x })
                    .ToList(),
                Options = options,
                Pager = pager
            };

            // maintain previous route data when generating page links
            var routeData = new RouteData();
            routeData.Values.Add("Options.Filter", options.Filter);
            routeData.Values.Add("Options.Search", options.Search);
            routeData.Values.Add("Options.Order", options.Order);

            pager.RouteData = routeData;

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("submit.BulkEdit")]
        public ActionResult Index(FormCollection input)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageUsers, "Not authorized to manage users"))
                return new HttpUnauthorizedResult();

            var viewModel = new UsersIndexViewModel { Users = new List<UserEntry>(), Options = new UserIndexOptions() };
            UpdateModel(viewModel);

            var checkedEntries = viewModel.Users.Where(c => c.IsChecked);
            switch (viewModel.Options.BulkAction)
            {
                case UsersBulkAction.None:
                    break;
                case UsersBulkAction.Approve:
                    foreach (var entry in checkedEntries)
                    {
                        Approve(entry.User.Id);
                    }
                    break;
                case UsersBulkAction.Disable:
                    foreach (var entry in checkedEntries)
                    {
                        Moderate(entry.User.Id);
                    }
                    break;
                //case UsersBulkAction.ChallengeEmail:
                //    foreach (var entry in checkedEntries)
                //    {
                //        SendChallengeEmail(entry.User.Id);
                //    }
                //    break;
                case UsersBulkAction.Delete:
                    foreach (var entry in checkedEntries)
                    {
                        Delete(entry.User.Id);
                    }
                    break;
            }

            return RedirectToAction("Index", ControllerContext.RouteData.Values);
        }

        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageUsers, "Not authorized to manage users"))
                return new HttpUnauthorizedResult();

            var model = new UserCreateViewModel();
            return View(model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST(UserCreateViewModel createModel)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageUsers, "Not authorized to manage users"))
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                if (!_userService.VerifyUserUnicity(createModel.UserName, createModel.Email))
                {
                    ModelState.AddModelError("UserName", "User with that username and/or email already exists.");
                }
            }

            if (ModelState.IsValid)
            {
                var user = _membershipService.CreateUser(new CreateUserParams(
                                                   createModel.UserName,
                                                   createModel.Password,
                                                   createModel.Email, true));
                Services.Notifier.Information("User created");
                return RedirectToAction("Index");
            }

            Services.TransactionManager.Cancel();
            return View(createModel);
        }

        public ActionResult Edit(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageUsers, "Not authorized to manage users"))
                return new HttpUnauthorizedResult();

            var user = _userRepository.Get(id);
            var model = new UserEditViewModel { Id = user.Id, Email = user.Email, UserName = user.UserName };
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(UserEditViewModel model)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageUsers, "Not authorized to manage users"))
                return new HttpUnauthorizedResult();

            var user = _userRepository.Get(model.Id);
            string previousName = user.UserName;


            if (ModelState.IsValid && !_userService.VerifyUserUnicity(model.Id, model.UserName, model.Email))
            {
                ModelState.AddModelError("UserName", "User with that username and/or email already exists.");
            }

            ///also update the Super user if this is the renamed account
            if (ModelState.IsValid && String.Equals(Services.WorkContext.CurrentSite.SuperUser, previousName, StringComparison.Ordinal))
            {
                var siteSetting = _settingService.LoadSetting<SiteSettings>(); ;
                siteSetting.SuperUser = model.UserName;
                _settingService.SaveSetting(siteSetting);
            }

            if (ModelState.IsValid)
            {
                user.Email = model.Email;
                user.UserName = model.UserName;

                user.Email = model.Email;
                user.UserName = model.UserName.ToLowerInvariant();

                Services.Notifier.Information("User information updated");
                return RedirectToAction("Index");
            }

            Services.TransactionManager.Cancel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageUsers, "Not authorized to manage users"))
                return new HttpUnauthorizedResult();

            var user = _userRepository.Get(id);

            if (user != null)
            {
                if (String.Equals(Services.WorkContext.CurrentSite.SuperUser, user.UserName, StringComparison.Ordinal))
                {
                    Services.Notifier.Error("The Super user can't be removed. Please disable this account or specify another Super user account.");
                }
                else if (String.Equals(Services.WorkContext.CurrentUser.UserName, user.UserName, StringComparison.Ordinal))
                {
                    Services.Notifier.Error("You can't remove your own account. Please log in with another account.");
                }
                else
                {
                    _userRepository.Delete(_userRepository.Get(id));
                    Services.Notifier.Information(string.Format("User {0} deleted", user.UserName));
                }
            }

            return RedirectToAction("Index");
        }



        [HttpPost]
        public ActionResult Approve(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageUsers, "Not authorized to manage users"))
                return new HttpUnauthorizedResult();

            var user = _userRepository.Get(id);

            if (user != null)
            {
                user.RegistrationStatus = UserStatus.Approved;
                Services.Notifier.Information(string.Format("User {0} approved", user.UserName));
                _userEventHandlers.Approved(user);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Moderate(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageUsers, "Not authorized to manage users"))
                return new HttpUnauthorizedResult();

            var user = _userRepository.Get(id);

            if (user != null)
            {
                if (String.Equals(Services.WorkContext.CurrentUser.UserName, user.UserName, StringComparison.Ordinal))
                {
                    Services.Notifier.Error("You can't disable your own account. Please log in with another account");
                }
                else
                {
                    user.RegistrationStatus = UserStatus.Pending;
                    Services.Notifier.Information(string.Format("User {0} disabled", user.UserName));
                }
            }

            return RedirectToAction("Index");
        }

        public void AddModelError(string key, string errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }



    }

}
