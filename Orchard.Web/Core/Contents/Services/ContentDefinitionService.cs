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
            return _contentDefinitionManager.ListTypeDefinitions().Select(ctd => new EditTypeViewModel(ctd)).OrderBy(m => m.DisplayName);
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

            foreach (var part in viewModel.Parts)
            {
                part._Definition.ContentTypeDefinition = contentTypeDefinition;
                //part.Templates = _contentDefinitionEditorEvents.TypePartEditor(part._Definition);
            }

            return viewModel;
        }

        public ContentTypeDefinition AddType(string name, string displayName)
        {
            if (String.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("displayName");
            }

            if (String.IsNullOrWhiteSpace(name))
            {
                name = GenerateContentTypeNameFromDisplayName(displayName);
            }
            else
            {
                if (!name[0].IsLetter())
                {
                    throw new ArgumentException("Content type name must start with a letter", "name");
                }
            }

            //while (_contentDefinitionManager.GetTypeDefinition(name) != null)
            //    name = VersionName(name);

            var contentTypeDefinition = new ContentTypeDefinition(name, displayName);
            _contentDefinitionManager.StoreTypeDefinition(contentTypeDefinition);
            //_contentDefinitionManager.AlterTypeDefinition(name, cfg => cfg.Creatable().Draftable().Listable().Securable());

            return contentTypeDefinition;
        }

        public void AlterType(EditTypeViewModel typeViewModel, ContentManagement.IUpdateModel updateModel)
        {
            var updater = new Updater(updateModel);
            _contentDefinitionManager.AlterTypeDefinition(typeViewModel.Name, typeBuilder =>
            {
                typeBuilder.DisplayedAs(typeViewModel.DisplayName);

                foreach (var part in typeViewModel.Parts)
                {
                    var partViewModel = part;

                    // enable updater to be aware of changing part prefix
                    updater._prefix = secondHalf => String.Format("{0}.{1}", partViewModel.Prefix, secondHalf);

                    //// allow extensions to alter typePart configuration
                    //typeBuilder.WithPart(partViewModel.PartDefinition.Name, typePartBuilder =>
                    //{
                    //    partViewModel.Templates = _contentDefinitionEditorEvents.TypePartEditorUpdate(typePartBuilder, updater);
                    //});

                    //_contentDefinitionManager.AlterPartDefinition(partViewModel.PartDefinition.Name, partBuilder =>
                    //{
                    //    var fieldFirstHalf = String.Format("{0}.{1}", partViewModel.Prefix, partViewModel.PartDefinition.Prefix);
                    //    foreach (var field in partViewModel.PartDefinition.Fields)
                    //    {
                    //        var fieldViewModel = field;

                    //        // enable updater to be aware of changing field prefix
                    //        updater._prefix = secondHalf =>
                    //            String.Format("{0}.{1}.{2}", fieldFirstHalf, fieldViewModel.Prefix, secondHalf);
                    //        // allow extensions to alter partField configuration
                    //        partBuilder.WithField(fieldViewModel.Name, partFieldBuilder =>
                    //        {
                    //            fieldViewModel.Templates = _contentDefinitionEditorEvents.PartFieldEditorUpdate(partFieldBuilder, updater);
                    //        });
                    //    }
                    //});
                }
            });
        }

        public void RemoveType(string name, bool deleteContent)
        {
            // first remove all attached parts
            var typeDefinition = _contentDefinitionManager.GetTypeDefinition(name);
            var partDefinitions = typeDefinition.Parts.ToArray();
            foreach (var partDefinition in partDefinitions)
            {
                RemovePartFromType(partDefinition.PartDefinition.Name, name);
            }

            _contentDefinitionManager.DeleteTypeDefinition(name);

            // delete all content items (but keep versions)
            if (deleteContent)
            {
                //name
                var contentItems = Services.ContentManager.Query().List();
                foreach (var contentItem in contentItems)
                {
                    Services.ContentManager.Remove(contentItem);
                }
            }
        }

        public void AddPartToType(string partName, string typeName)
        {
            _contentDefinitionManager.AlterTypeDefinition(typeName, typeBuilder => typeBuilder.WithPart(partName));
        }

        public void RemovePartFromType(string partName, string typeName)
        {
            _contentDefinitionManager.AlterTypeDefinition(typeName, typeBuilder => typeBuilder.RemovePart(partName));
        }

        public string GenerateContentTypeNameFromDisplayName(string displayName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EditPartViewModel> GetParts()
        {
            var typeNames = new HashSet<string>(GetTypes().Select(ctd => ctd.Name));

            // user-defined parts
            // except for those parts with the same name as a type (implicit type's part or a mistake)
            //var userContentParts = _contentDefinitionManager.ListPartDefinitions()
            //    .Where(cpd => !typeNames.Contains(cpd.Name))
            //    .Select(cpd => new EditPartViewModel(cpd))
            //    .ToDictionary(
            //        k => k.Name,
            //        v => v);

            // code-defined parts
            var codeDefinedParts = _contentPartDrivers
                    .SelectMany(d => d.GetPartInfo()
                        .Where(cpd => !userContentParts.ContainsKey(cpd.PartName))
                        .Select(cpi => new EditPartViewModel { Name = cpi.PartName, DisplayName = cpi.PartName }))
                    .ToList();

            // Order by display name
            return codeDefinedParts
                .Union(userContentParts.Values)
                .OrderBy(m => m.DisplayName);
        }

        public EditPartViewModel GetPart(string name)
        {
            var contentPartDefinition = _contentDefinitionManager.GetPartDefinition(name);

            if (contentPartDefinition == null)
                return null;

            var viewModel = new EditPartViewModel(contentPartDefinition)
            {
                //Templates = _contentDefinitionEditorEvents.PartEditor(contentPartDefinition)
            };

            return viewModel;
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