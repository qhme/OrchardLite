using Orchard.ContentManagement.MetaData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.MetaData.Builders
{
    public class ContentTypeDefinitionBuilder
    {
        private string _name;
        private readonly IList<ContentTypePartDefinition> _parts;

        public ContentTypeDefinition Current { get; private set; }

        public ContentTypeDefinitionBuilder()
            : this(new ContentTypeDefinition(null, null))
        {
        }

        public ContentTypeDefinitionBuilder(ContentTypeDefinition existing)
        {
            Current = existing;

            if (existing == null)
            {
                _parts = new List<ContentTypePartDefinition>();
            }
            else
            {
                _name = existing.Name;
                _parts = existing.Parts.ToList();
            }
        }

        public ContentTypeDefinition Build()
        {
            return new ContentTypeDefinition(_name, _parts);
        }

        public ContentTypeDefinitionBuilder Named(string name)
        {
            _name = name;
            return this;
        }

        public ContentTypeDefinitionBuilder DisplayedAs(string displayName)
        {
            return this;
        }

        public ContentTypeDefinitionBuilder WithSetting(string name, string value)
        {
            return this;
        }

        public ContentTypeDefinitionBuilder RemovePart(string partName)
        {
            var existingPart = _parts.SingleOrDefault(x => x.PartName == partName);
            if (existingPart != null)
            {
                _parts.Remove(existingPart);
            }
            return this;
        }

        public ContentTypeDefinitionBuilder WithPart(string partName, int index)
        {
            var existingPart = _parts.SingleOrDefault(x => x.PartName == partName);
            if (existingPart != null)
            {
                _parts.Remove(existingPart);
            }
            else
            {
                existingPart = new ContentTypePartDefinition(partName);
            }

            existingPart.Index = index;
            _parts.Add(existingPart);
            return this;
        }
    }

}
