using Orchard.ContentManagement.MetaData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.Core.Contents.ViewModels
{
    public class EditTypeViewModel
    {
        public EditTypeViewModel()
        {
            Settings = new SettingsDictionary();
            Parts = new List<EditTypePartViewModel>();
        }

        public EditTypeViewModel(ContentTypeDefinition contentTypeDefinition)
        {
            Name = contentTypeDefinition.Name;
            DisplayName = contentTypeDefinition.DisplayName;
            Settings = contentTypeDefinition.Settings;
            Parts = GetTypeParts(contentTypeDefinition).ToList();
            _Definition = contentTypeDefinition;
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public SettingsDictionary Settings { get; set; }
        public IEnumerable<EditTypePartViewModel> Parts { get; set; }
        public ContentTypeDefinition _Definition { get; private set; }

        private IEnumerable<EditTypePartViewModel> GetTypeParts(ContentTypeDefinition contentTypeDefinition)
        {
            return contentTypeDefinition.Parts
                .Where(p => !string.Equals(p.PartName, Name, StringComparison.OrdinalIgnoreCase))
                .Select((p, i) => new EditTypePartViewModel(i, p) { Type = this });
        }
    }
}