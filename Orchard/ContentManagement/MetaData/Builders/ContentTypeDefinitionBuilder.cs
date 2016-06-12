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
        private string _displayName;
        private readonly IList<ContentTypePartDefinition> _parts;
        private readonly SettingsDictionary _settings;

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
                _settings = new SettingsDictionary();
            }
            else
            {
                _name = existing.Name;
                _displayName = existing.DisplayName;
                _parts = existing.Parts.ToList();
                _settings = new SettingsDictionary(existing.Settings.ToDictionary(kv => kv.Key, kv => kv.Value));
            }
        }

        public ContentTypeDefinition Build()
        {
            return new ContentTypeDefinition(_name, _displayName, _parts, _settings);
        }

        public ContentTypeDefinitionBuilder Named(string name)
        {
            _name = name;
            return this;
        }

        public ContentTypeDefinitionBuilder DisplayedAs(string displayName)
        {
            _displayName = displayName;
            return this;
        }

        public ContentTypeDefinitionBuilder WithSetting(string name, string value)
        {
            _settings[name] = value;
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


        public ContentTypeDefinitionBuilder WithPart(string partName)
        {
            var existingPart = _parts.SingleOrDefault(x => x.PartName == partName);
            if (existingPart != null)
            {
                _parts.Remove(existingPart);
            }
            else
            {
                existingPart = new ContentTypePartDefinition(partName, new SettingsDictionary());
            }

            _parts.Add(existingPart);
            return this;
        }
    }

}
