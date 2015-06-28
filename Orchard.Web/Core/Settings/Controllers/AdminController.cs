using Orchard.Core.Settings;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Orchard.Settings;
using Orchard.UI.Notify;

namespace Orchard.Core.Settings.Controllers
{
    [ValidateInput(false)]
    public class AdminController : Controller
    {
        private readonly ISiteService _siteService;
        public IOrchardServices Services { get; private set; }

        public AdminController(ISiteService siteService,
            IOrchardServices services)
        {
            _siteService = siteService;
            Services = services;
        }


        public ActionResult Index(string groupInfoId)
        {
            //if (!Services.Authorizer.Authorize(Permissions.ManageSettings,"Not authorized to manage settings"))
            //    return new HttpUnauthorizedResult();

            //dynamic model;
            //var site = _siteService.GetSiteSettings();
            //if (!string.IsNullOrWhiteSpace(groupInfoId))
            //{
            //    model = Services.ContentManager.BuildEditor(site, groupInfoId);

            //    if (model == null)
            //        return HttpNotFound();

            //    var groupInfo = Services.ContentManager.GetEditorGroupInfo(site, groupInfoId);
            //    if (groupInfo == null)
            //        return HttpNotFound();

            //    model.GroupInfo = groupInfo;
            //}
            //else
            //{
            //    model = Services.ContentManager.BuildEditor(site);
            //}

            //return View(model);
            return View();
        }

        [HttpPost, ActionName("Index")]
        public ActionResult IndexPOST(string groupInfoId)
        {
            //if (!Services.Authorizer.Authorize(Permissions.ManageSettings,"Not authorized to manage settings"))
            //    return new HttpUnauthorizedResult();

            //var site = _siteService.GetSiteSettings();
            //var model = Services.ContentManager.UpdateEditor(site, this, groupInfoId);

            //GroupInfo groupInfo = null;

            //if (!string.IsNullOrWhiteSpace(groupInfoId))
            //{
            //    if (model == null)
            //    {
            //        Services.TransactionManager.Cancel();
            //        return HttpNotFound();
            //    }

            //    groupInfo = Services.ContentManager.GetEditorGroupInfo(site, groupInfoId);
            //    if (groupInfo == null)
            //    {
            //        Services.TransactionManager.Cancel();
            //        return HttpNotFound();
            //    }
            //}

            //if (!ModelState.IsValid)
            //{
            //    Services.TransactionManager.Cancel();
            //    model.GroupInfo = groupInfo;

            //    return View(model);
            //}

            //Services.Notifier.Information("Settings updated");
            return RedirectToAction("Index");
        }
    }
}
