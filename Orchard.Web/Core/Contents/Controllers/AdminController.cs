using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.Records;
using Orchard.Core.Contents.ViewModels;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Settings;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Orchard.Core.Contents.Controllers
{
    [ValidateInput(false)]
    public class AdminController : Controller, IUpdateModel
    {
        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly ITransactionManager _transactionManager;
        private readonly ISiteService _siteService;
        public IOrchardServices Services { get; private set; }

        public ILogger Logger { get; set; }

        public AdminController(IOrchardServices orchardServices,
            IContentManager contentManager, IContentDefinitionManager contentDefinitionManager,
            ITransactionManager transactionManager, ISiteService siteService)
        {
            Services = orchardServices;
            _contentManager = contentManager;
            _contentDefinitionManager = contentDefinitionManager;
            _transactionManager = transactionManager;
            _siteService = siteService;

            Logger = NullLogger.Instance;
        }

        public ActionResult List(ListContentsViewModel model, PagerParameters pagerParameters)
        {
            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            var query = _contentManager.Query();

            //if (!string.IsNullOrEmpty(model.TypeName))
            //{
            //    var contentTypeDefinition = _contentDefinitionManager.GetTypeDefinition(model.TypeName);
            //    if (contentTypeDefinition == null)
            //        return HttpNotFound();

            //    model.TypeDisplayName = !string.IsNullOrWhiteSpace(contentTypeDefinition.DisplayName)
            //                                ? contentTypeDefinition.DisplayName
            //                                : contentTypeDefinition.Name;
            //    query = query.ForType(model.TypeName);
            //}

            //query=query.OrderByDescending<ContentItemRecord>()
            //switch (model.Options.OrderBy)
            //{
            //    case ContentsOrder.Modified:
            //        //query = query.OrderByDescending<ContentPartRecord, int>(ci => ci.ContentItemRecord.Versions.Single(civr => civr.Latest).Id);
            //        query = query.OrderByDescending<CommonPartRecord>(cr => cr.ModifiedUtc);
            //        break;
            //    case ContentsOrder.Published:
            //        query = query.OrderByDescending<CommonPartRecord>(cr => cr.PublishedUtc);
            //        break;
            //    case ContentsOrder.Created:
            //        //query = query.OrderByDescending<ContentPartRecord, int>(ci => ci.Id);
            //        query = query.OrderByDescending<CommonPartRecord>(cr => cr.CreatedUtc);
            //        break;
            //}

            var maxPagedCount = _siteService.GetSiteSettings().MaxPagedCount;
            if (maxPagedCount > 0 && pager.PageSize > maxPagedCount)
                pager.PageSize = maxPagedCount;
            pager.Total = query.Count();
            var pageOfContentItems = query.Slice(pager.GetStartIndex(), pager.PageSize).ToList();

            model.Pager = pager;
            model.Items = pageOfContentItems;
            return View(model);
        }



        public new bool TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) where TModel : class
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        public void AddModelError(string key, Localization.LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}