using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard.Data.Migration;
using Orchard.Environment.Configuration;
using Orchard.Environment.Descriptor.Models;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Models;
using Orchard.Environment.Features;
using Orchard.Logging;
using Orchard.Modules.Events;
using Orchard.Modules.Models;
using Orchard.Modules.Services;
using Orchard.Modules.ViewModels;
using Orchard.Mvc;
using Orchard.Mvc.Extensions;
using Orchard.Security;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Settings;

namespace Orchard.Modules.Controllers
{
    public class AdminController : Controller
    {
        private readonly IExtensionDisplayEventHandler _extensionDisplayEventHandler;
        private readonly IModuleService _moduleService;
        private readonly IDataMigrationManager _dataMigrationManager;
        private readonly IExtensionManager _extensionManager;
        private readonly IFeatureManager _featureManager;
        private readonly ShellDescriptor _shellDescriptor;
        private readonly ShellSettings _shellSettings;
        private readonly ISiteService _siteService;


        public AdminController(
            IEnumerable<IExtensionDisplayEventHandler> extensionDisplayEventHandlers,
            IOrchardServices services,
            IModuleService moduleService,
             ISiteService siteService,
            IDataMigrationManager dataMigrationManager,
            IExtensionManager extensionManager,
            IFeatureManager featureManager,
             ShellDescriptor shellDescriptor,
            ShellSettings shellSettings
            )
        {
            Services = services;
            _extensionDisplayEventHandler = extensionDisplayEventHandlers.FirstOrDefault();
            _moduleService = moduleService;
            _dataMigrationManager = dataMigrationManager;
            _extensionManager = extensionManager;
            _featureManager = featureManager;
            _shellDescriptor = shellDescriptor;
            _shellSettings = shellSettings;
            _siteService = siteService; ;

            Logger = NullLogger.Instance;
        }

        public IOrchardServices Services { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult Index(ModulesIndexOptions options, PagerParameters pagerParameters)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, "Not allowed to manage modules"))
                return new HttpUnauthorizedResult();

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            IEnumerable<ModuleEntry> modules = _extensionManager.AvailableExtensions()
                .Where(extensionDescriptor => DefaultExtensionTypes.IsModule(extensionDescriptor.ExtensionType) &&

                                              (string.IsNullOrEmpty(options.SearchText) || extensionDescriptor.Name.ToLowerInvariant().Contains(options.SearchText.ToLowerInvariant())))
                .OrderBy(extensionDescriptor => extensionDescriptor.Name)
                .Select(extensionDescriptor => new ModuleEntry { Descriptor = extensionDescriptor });

            pager.Total = modules.Count();


            // This way we can more or less reliably handle this implicit dependency.
            var installModules = _featureManager.GetEnabledFeatures().FirstOrDefault(f => f.Id == "PackagingServices") != null;

            modules = modules.ToList();
            foreach (ModuleEntry moduleEntry in modules)
            {
                moduleEntry.IsRecentlyInstalled = _moduleService.IsRecentlyInstalled(moduleEntry.Descriptor);
                moduleEntry.CanUninstall = installModules;

                if (_extensionDisplayEventHandler != null)
                {
                    foreach (string notification in _extensionDisplayEventHandler.Displaying(moduleEntry.Descriptor, ControllerContext.RequestContext))
                    {
                        moduleEntry.Notifications.Add(notification);
                    }
                }
            }

            return View(new ModulesIndexViewModel
            {
                Modules = modules,
                InstallModules = installModules,
                Options = options,
                Pager = pager
            });
        }

        public ActionResult Features()
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageFeatures, "Not allowed to manage features"))
                return new HttpUnauthorizedResult();

            var featuresThatNeedUpdate = _dataMigrationManager.GetFeaturesThatNeedUpdate();

            IEnumerable<ModuleFeature> features = _featureManager.GetAvailableFeatures()
                 .Select(f => new ModuleFeature
                {
                    Descriptor = f,
                    IsEnabled = _shellDescriptor.Features.Any(sf => sf.Name == f.Id),
                    IsRecentlyInstalled = _moduleService.IsRecentlyInstalled(f.Extension),
                    NeedsUpdate = featuresThatNeedUpdate.Contains(f.Id),
                    DependentFeatures = _moduleService.GetDependentFeatures(f.Id).Where(x => x.Id != f.Id).ToList()
                })
                .ToList();

            return View(new FeaturesViewModel
            {
                Features = features,
                IsAllowed = ModuleIsAllowed
            });
        }

        [HttpPost, ActionName("Features")]
        [FormValueRequired("submit.BulkExecute")]
        public ActionResult FeaturesPOST(FeaturesBulkAction bulkAction, IList<string> featureIds, bool? force)
        {

            if (!Services.Authorizer.Authorize(Permissions.ManageFeatures, "Not allowed to manage features"))
                return new HttpUnauthorizedResult();

            if (featureIds == null || !featureIds.Any())
            {
                ModelState.AddModelError("featureIds", "Please select one or more features.");
            }

            if (ModelState.IsValid)
            {
                var availableFeatures = _moduleService.GetAvailableFeatures().Where(feature => ModuleIsAllowed(feature.Descriptor.Extension)).ToList();
                var selectedFeatures = availableFeatures.Where(x => featureIds.Contains(x.Descriptor.Id)).ToList();
                var enabledFeatures = availableFeatures.Where(x => x.IsEnabled && featureIds.Contains(x.Descriptor.Id)).Select(x => x.Descriptor.Id).ToList();
                var disabledFeatures = availableFeatures.Where(x => !x.IsEnabled && featureIds.Contains(x.Descriptor.Id)).Select(x => x.Descriptor.Id).ToList();

                switch (bulkAction)
                {
                    case FeaturesBulkAction.None:
                        break;
                    case FeaturesBulkAction.Enable:
                        _moduleService.EnableFeatures(disabledFeatures, force == true);
                        break;
                    case FeaturesBulkAction.Disable:
                        _moduleService.DisableFeatures(enabledFeatures, force == true);
                        break;
                    case FeaturesBulkAction.Toggle:
                        _moduleService.EnableFeatures(disabledFeatures, force == true);
                        _moduleService.DisableFeatures(enabledFeatures, force == true);
                        break;
                    case FeaturesBulkAction.Update:
                        var featuresThatNeedUpdate = _dataMigrationManager.GetFeaturesThatNeedUpdate();
                        var selectedFeaturesThatNeedUpdate = selectedFeatures.Where(x => featuresThatNeedUpdate.Contains(x.Descriptor.Id));

                        foreach (var feature in selectedFeaturesThatNeedUpdate)
                        {
                            var id = feature.Descriptor.Id;
                            try
                            {
                                _dataMigrationManager.Update(id);
                                Services.Notifier.Information(string.Format("The feature {0} was updated successfully", id));
                            }
                            catch (Exception exception)
                            {
                                Services.Notifier.Error(string.Format("An error occured while updating the feature {0}: {1}", id, exception.Message));
                            }
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return RedirectToAction("Features");
        }

        /// <summary>
        /// Checks whether the module is allowed for the current tenant
        /// </summary>
        private bool ModuleIsAllowed(ExtensionDescriptor extensionDescriptor)
        {
            return _shellSettings.Modules.Length == 0 || _shellSettings.Modules.Contains(extensionDescriptor.Id);
        }
    }
}