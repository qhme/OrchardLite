using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Services;
using Orchard.Core.Contents.ViewModels;
using Orchard.Environment.Configuration;
using Orchard.Logging;
using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.UI.Notify;
using Orchard.Utility.Extensions;

namespace Orchard.Core.Contents.Controllers
{
    [Admin, ValidateInput(false)]
    public class ContentTypesController : Controller, IUpdateModel
    {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly Lazy<IEnumerable<IShellSettingsManagerEventHandler>> _settingsManagerEventHandlers;
        private readonly ShellSettings _settings;

        public ContentTypesController(
            IOrchardServices orchardServices,
            IContentDefinitionService contentDefinitionService,
            IContentDefinitionManager contentDefinitionManager,
            Lazy<IEnumerable<IShellSettingsManagerEventHandler>> settingsManagerEventHandlers,
            ShellSettings settings)
        {
            Services = orchardServices;
            _contentDefinitionService = contentDefinitionService;
            _contentDefinitionManager = contentDefinitionManager;
            _settingsManagerEventHandlers = settingsManagerEventHandlers;
            _settings = settings;
        }

        public IOrchardServices Services { get; private set; }

        public ILogger Logger { get; set; }

        public ActionResult Index()
        {
            if (!Services.Authorizer.Authorize(Permissions.ViewContentTypes, "Not allowed to view content types."))
                return new HttpUnauthorizedResult();

            return View(new ListContentTypesViewModel
            {
                Types = _contentDefinitionService.GetTypes()
            });
        }

        public ActionResult Edit(string id)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, "Not allowed to edit a content type."))
                return new HttpUnauthorizedResult();

            var typeViewModel = _contentDefinitionService.GetType(id);

            typeViewModel.AllParts = _contentDefinitionService.GetParts().Select(x => x.Name).ToList();
            if (typeViewModel == null)
                return HttpNotFound();

            return View(typeViewModel);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(string id, EditTypeViewModel model)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, "Not allowed to edit a content type."))
                return new HttpUnauthorizedResult();

            var typeViewModel = _contentDefinitionService.GetType(id);

            if (typeViewModel == null)
                return HttpNotFound();

            if (!ModelState.IsValid)
                return View(typeViewModel);

            _contentDefinitionService.AlterType(model, this);
            if (!ModelState.IsValid)
            {
                Services.TransactionManager.Cancel();
                return View(typeViewModel);
            }

            Services.Notifier.Information(string.Format("\"{0}\" settings have been saved.", typeViewModel.Name));
            return RedirectToAction("Index");
        }
 

        public ActionResult ListParts()
        {
            return View(new ListContentPartsViewModel
            {
                // only user-defined parts (not code as they are not configurable)
                Parts = _contentDefinitionService.GetParts()
            });
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return base.TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, Localization.LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}