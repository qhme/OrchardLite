using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Contents.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Utility.Extensions;
using Orchard.ContentManagement;

namespace Orchard.Core.Contents.Services
{
    public class ContentDefinitionService : IContentDefinitionService
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IEnumerable<IContentPartDriver> _contentPartDrivers;

        public ContentDefinitionService(IContentDefinitionManager contentDefinitionManager, IEnumerable<IContentPartDriver> contentPartDrivers,
              IOrchardServices services)
        {
            _contentDefinitionManager = contentDefinitionManager;
            _contentPartDrivers = contentPartDrivers;
            Services = services;
        }

        public IOrchardServices Services { get; set; }

        public IEnumerable<EditTypeViewModel> GetTypes()
        {
            return _contentDefinitionManager.ListTypeDefinitions().Select(ctd => new EditTypeViewModel(ctd)).OrderBy(m => m.Description);
        }

        public EditTypeViewModel GetType(string name)
        {
            var contentTypeDefinition = _contentDefinitionManager.GetTypeDefinition(name);

            if (contentTypeDefinition == null)
                return null;

            var viewModel = new EditTypeViewModel(contentTypeDefinition)
            {
                //Templates = _contentDefinitionEditorEvents.TypeEditor(contentTypeDefinition)
            };


            return viewModel;
        }


        public void AlterType(EditTypeViewModel typeViewModel, ContentManagement.IUpdateModel updateModel)
        {
            var updater = new Updater(updateModel);
            _contentDefinitionManager.AlterTypeDefinition(typeViewModel.Name, typeBuilder =>
            {
                typeBuilder.DisplayedAs(typeViewModel.Description);

                foreach (var part in typeViewModel.Parts)
                {
                    // enable updater to be aware of changing part prefix
                    updater._prefix = secondHalf => String.Format("{0}.{1}", part.Prefix, secondHalf);
                    typeBuilder.WithPart(part.PartName, part.Index);
                }
            });
        }


        public IEnumerable<EditPartViewModel> GetParts()
        {
            // code-defined parts
            var codeDefinedParts = _contentPartDrivers
                    .SelectMany(d => d.GetPartInfo()
                        //.Where(cpd => !userContentParts.ContainsKey(cpd.PartName))
                        .Select(cpi => new EditPartViewModel(cpi.PartName)))
                    .ToList();

            // Order by display name
            return codeDefinedParts;
        }


        class Updater : IUpdateModel
        {
            private readonly IUpdateModel _thunk;

            public Updater(IUpdateModel thunk)
            {
                _thunk = thunk;
            }

            public Func<string, string> _prefix = x => x;

            public bool TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) where TModel : class
            {
                return _thunk.TryUpdateModel(model, _prefix(prefix), includeProperties, excludeProperties);
            }


            public void AddModelError(string key, Localization.LocalizedString errorMessage)
            {
                _thunk.AddModelError(_prefix(key), errorMessage);
            }
        }
    }
}