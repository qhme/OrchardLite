using Orchard.Caching;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.MetaData.Services;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Orchard.Logging;
using Orchard.ContentManagement.Drivers;

namespace Orchard.Core.Settings.Metadata
{
    public class ContentDefinitionManager : Component, IContentDefinitionManager
    {
        private const string ContentDefinitionSignal = "ContentDefinitionManager";
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        private readonly ISettingsFormatter _settingsFormatter;
        private readonly IRepository<ContentTypePartDefinitionRecord> _typePartDefinitionRecord;
        private readonly IEnumerable<IContentTypeDefinition> _typeDefinitions;

        public ContentDefinitionManager(ICacheManager cacheManager, IRepository<ContentTypePartDefinitionRecord> typePartDefinitionRecord,
            IEnumerable<IContentTypeDefinition> typeDefintions,
            ISignals signals, ISettingsFormatter settingsFormatter)
        {
            _cacheManager = cacheManager;
            _signals = signals;
            _settingsFormatter = settingsFormatter;
            _typePartDefinitionRecord = typePartDefinitionRecord;
            _typeDefinitions = typeDefintions;
        }


        public IEnumerable<ContentTypeDefinition> ListTypeDefinitions()
        {
            return AcquireContentTypeDefinitions().Values;
        }

        public ContentTypeDefinition GetTypeDefinition(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var contentTypeDefinitions = AcquireContentTypeDefinitions();
            if (contentTypeDefinitions.ContainsKey(name))
            {
                return contentTypeDefinitions[name];
            }

            return null;
        }


        public void StoreTypeDefinition(ContentTypeDefinition contentTypeDefinition)
        {
            Apply(contentTypeDefinition);
            TriggerContentDefinitionSignal();
        }

        private void MonitorContentDefinitionSignal(AcquireContext<string> ctx)
        {
            ctx.Monitor(_signals.When(ContentDefinitionSignal));
        }

        private void TriggerContentDefinitionSignal()
        {
            _signals.Trigger(ContentDefinitionSignal);
        }

        private IDictionary<string, ContentTypeDefinition> AcquireContentTypeDefinitions()
        {
            return _cacheManager.Get("ContentTypeDefinitions", ctx =>
            {
                MonitorContentDefinitionSignal(ctx);

                var codedTypes = _typeDefinitions.Select(x => new { x.TypeName, x.Description });

                var contentTypeDefinitions = _typePartDefinitionRecord.Table.ToList().GroupBy(x => x.TypeName).Select(g => Build(g.Key, g)).ToList();
                foreach (var coded in codedTypes)
                {
                    var definitioned = contentTypeDefinitions.FirstOrDefault(x => x.Name == coded.TypeName);
                    if (definitioned != null)
                        definitioned.Description = coded.Description;
                    else
                    {
                        var newDTD = Build(coded.TypeName, Enumerable.Empty<ContentTypePartDefinitionRecord>());
                        newDTD.Description = coded.Description;
                        contentTypeDefinitions.Add(newDTD);
                    }
                }

                return contentTypeDefinitions.ToDictionary(x => x.Name, y => y, StringComparer.OrdinalIgnoreCase);
            });
        }

        private void Apply(ContentTypeDefinition model)
        {
            var toRemove = _typePartDefinitionRecord.Table.Where(p => p.TypeName == model.Name).ToList().Where(p =>
                model.Parts.All(part => p.PartName != part.PartName))
               .ToList();

            foreach (var remove in toRemove)
            {
                _typePartDefinitionRecord.Delete(remove);
            }

            foreach (var part in model.Parts.Where(x => !string.IsNullOrEmpty(x.PartName)))
            {
                var partName = part.PartName;
                var typePartRecord = _typePartDefinitionRecord.Table.SingleOrDefault(r => r.TypeName == model.Name && r.PartName == partName);
                if (typePartRecord == null)
                {
                    typePartRecord = new ContentTypePartDefinitionRecord { PartName = partName, TypeName = model.Name, Ord = part.Index };
                    _typePartDefinitionRecord.Create(typePartRecord);
                }
                else
                    typePartRecord.Ord = part.Index;
            }

        }

        //private void Apply(ContentTypePartDefinition model, ContentTypePartDefinitionRecord record)
        //{
        //    record.Settings = Compose(_settingsFormatter.Map(model.Settings));
        //}


        ContentTypeDefinition Build(string typeName, IEnumerable<ContentTypePartDefinitionRecord> typeParts)
        {
            return new ContentTypeDefinition(typeName, typeParts.Select(Build));
        }

        ContentTypePartDefinition Build(ContentTypePartDefinitionRecord source)
        {
            var cpd = new ContentTypePartDefinition(source.PartName);
            cpd.Index = source.Ord;
            return cpd;
        }

        XElement Parse(string settings)
        {
            if (string.IsNullOrEmpty(settings))
                return null;

            try
            {
                return XElement.Parse(settings);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unable to parse settings xml");
                return null;
            }
        }

        static string Compose(XElement map)
        {
            if (map == null)
                return null;

            return map.ToString();
        }

    }
}