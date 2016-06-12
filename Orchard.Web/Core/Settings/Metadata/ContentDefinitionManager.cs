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

namespace Orchard.Core.Settings.Metadata
{
    public class ContentDefinitionManager : Component, IContentDefinitionManager
    {
        private const string ContentDefinitionSignal = "ContentDefinitionManager";
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        private readonly IRepository<ContentTypeDefinitionRecord> _typeDefinitionRepository;
        private readonly ISettingsFormatter _settingsFormatter;

        public ContentDefinitionManager(ICacheManager cacheManager,
            ISignals signals,
            IRepository<ContentTypeDefinitionRecord> typeDefinitionRepository, ISettingsFormatter settingsFormatter)
        {
            _cacheManager = cacheManager;
            _signals = signals;
            _typeDefinitionRepository = typeDefinitionRepository;
            _settingsFormatter = settingsFormatter;
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


        public void DeleteTypeDefinition(string name)
        {
            var record = _typeDefinitionRepository.Table.SingleOrDefault(x => x.Name == name);

            // deletes the content type record associated
            if (record != null)
            {
                _typeDefinitionRepository.Delete(record);
            }

            // invalidates the cache
            TriggerContentDefinitionSignal();
        }

        public void StoreTypeDefinition(ContentTypeDefinition contentTypeDefinition)
        {
            Apply(contentTypeDefinition, Acquire(contentTypeDefinition));
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

                var contentTypeDefinitionRecords = _typeDefinitionRepository.Table
                   .FetchMany(x => x.ContentTypePartDefinitionRecords)
                    .Select(Build);

                return contentTypeDefinitionRecords.ToDictionary(x => x.Name, y => y, StringComparer.OrdinalIgnoreCase);
            });
        }



        private ContentTypeDefinitionRecord Acquire(ContentTypeDefinition contentTypeDefinition)
        {
            var result = _typeDefinitionRepository.Table.SingleOrDefault(x => x.Name == contentTypeDefinition.Name);
            if (result == null)
            {
                result = new ContentTypeDefinitionRecord { Name = contentTypeDefinition.Name, DisplayName = contentTypeDefinition.DisplayName };
                _typeDefinitionRepository.Create(result);
            }
            return result;
        }



        private void Apply(ContentTypeDefinition model, ContentTypeDefinitionRecord record)
        {
            record.DisplayName = model.DisplayName;
            record.Settings = _settingsFormatter.Map(model.Settings).ToString();

            var toRemove = record.ContentTypePartDefinitionRecords
                .Where(partDefinitionRecord => model.Parts.All(part => partDefinitionRecord.PartName != part.PartName))
                .ToList();

            foreach (var remove in toRemove)
            {
                record.ContentTypePartDefinitionRecords.Remove(remove);
            }

            foreach (var part in model.Parts)
            {
                var partName = part.PartName;
                var typePartRecord = record.ContentTypePartDefinitionRecords.SingleOrDefault(r => r.PartName == partName);
                if (typePartRecord == null)
                {
                    typePartRecord = new ContentTypePartDefinitionRecord { PartName = partName };
                    record.ContentTypePartDefinitionRecords.Add(typePartRecord);
                }
                Apply(part, typePartRecord);
            }
        }

        private void Apply(ContentTypePartDefinition model, ContentTypePartDefinitionRecord record)
        {
            record.Settings = Compose(_settingsFormatter.Map(model.Settings));
        }



        ContentTypeDefinition Build(ContentTypeDefinitionRecord source)
        {
            return new ContentTypeDefinition(
                source.Name,
                source.DisplayName,
                source.ContentTypePartDefinitionRecords.Select(Build),
                _settingsFormatter.Map(Parse(source.Settings)));
        }

        ContentTypePartDefinition Build(ContentTypePartDefinitionRecord source)
        {
            return new ContentTypePartDefinition(source.PartName,  _settingsFormatter.Map(Parse(source.Settings)));
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