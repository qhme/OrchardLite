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
            Parts = new List<EditTypePartViewModel>();
        }

        public EditTypeViewModel(ContentTypeDefinition contentTypeDefinition)
        {
            Name = contentTypeDefinition.Name;
            Parts = GetTypeParts(contentTypeDefinition).ToList();
            Description = contentTypeDefinition.Description;
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public List<string> AllParts { get; set; }

        public IEnumerable<EditTypePartViewModel> Parts { get; set; }

        private IEnumerable<EditTypePartViewModel> GetTypeParts(ContentTypeDefinition contentTypeDefinition)
        {
            return contentTypeDefinition.Parts
                .Where(p => !string.Equals(p.PartName, Name, StringComparison.OrdinalIgnoreCase))
                .Select((p, i) => new EditTypePartViewModel(i, p) { Index = p.Index, Type = this });
        }
    }
}