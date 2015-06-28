using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Caching;
using Orchard.Core.Settings.Models;
using Orchard.Data;
using Orchard.Settings;

namespace Orchard.Core.Settings.Services
{
    public class SettingService : ISettingService
    {
        private const string SETTINGS_ALL_KEY = "settings.all";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string SETTINGS_PATTERN_KEY = "settings.";

        private readonly IRepository<SettingRecord> _settingRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;


        public SettingService(ICacheManager cacheManager,
            ISignals signals,
            IRepository<SettingRecord> settingRepository)
        {
            this._cacheManager = cacheManager;
            this._signals = signals;
            this._settingRepository = settingRepository;
        }

        public T LoadSetting<T>() where T : ISettings, new()
        {
            var settings = Activator.CreateInstance<T>();

            foreach (var prop in typeof(T).GetProperties())
            {
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = typeof(T).Name + "." + prop.Name;
                //load by store
                string setting = GetSettingByKey<string>(key, loadSharedValueIfNotFound: true);
                if (setting == null)
                    continue;

                if (!CommonHelper.GetCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!CommonHelper.GetCustomTypeConverter(prop.PropertyType).IsValid(setting))
                    continue;

                object value = CommonHelper.GetCustomTypeConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                //set property
                prop.SetValue(settings, value, null);
            }

            return settings;
        }

        public void SaveSetting<T>(T settings) where T : ISettings, new()
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (!CommonHelper.GetCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                string key = typeof(T).Name + "." + prop.Name;
                //Duck typing is not supported in C#. That's why we're using dynamic type
                dynamic value = prop.GetValue(settings, null);
                if (value != null)
                    SetSetting(key, value, false);
                else
                    SetSetting(key, "", false);
            }

            _signals.Trigger(SETTINGS_ALL_KEY);
        }

        public void DeleteSetting<T>() where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        #region utils

        #region Nested classes

        [Serializable]
        public class SettingForCaching
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
        }

        #endregion

        protected IDictionary<string, IList<SettingForCaching>> GetAllSettingsCached()
        {
            //cache
            string key = string.Format(SETTINGS_ALL_KEY);
            return _cacheManager.Get(key, ctx =>
            {
                ctx.Monitor(_signals.When(SETTINGS_ALL_KEY));
                var query = from s in _settingRepository.Table
                            orderby s.Name
                            select s;
                var settings = query.ToList();
                var dictionary = new Dictionary<string, IList<SettingForCaching>>();
                foreach (var s in settings)
                {
                    var resourceName = s.Name.ToLowerInvariant();
                    var settingForCaching = new SettingForCaching()
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Value = s.Value
                    };
                    if (!dictionary.ContainsKey(resourceName))
                    {
                        //first setting
                        dictionary.Add(resourceName, new List<SettingForCaching>()
                        {
                            settingForCaching
                        });
                    }
                    else
                    {
                        dictionary[resourceName].Add(settingForCaching);
                    }
                }
                return dictionary;
            });
        }


        private T GetSettingByKey<T>(string key, T defaultValue = default(T), bool loadSharedValueIfNotFound = false)
        {
            if (String.IsNullOrEmpty(key))
                return defaultValue;

            var settings = GetAllSettingsCached();
            key = key.Trim().ToLowerInvariant();
            if (settings.ContainsKey(key))
            {
                var settingsByKey = settings[key];
                var setting = settingsByKey.FirstOrDefault();

                if (setting != null)
                    return CommonHelper.To<T>(setting.Value);
            }

            return defaultValue;
        }


        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void SetSetting<T>(string key, T value, bool clearCache = true)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            key = key.Trim().ToLowerInvariant();
            string valueStr = CommonHelper.GetCustomTypeConverter(typeof(T)).ConvertToInvariantString(value);

            var allSettings = GetAllSettingsCached();
            var settingForCaching = allSettings.ContainsKey(key) ?
                allSettings[key].FirstOrDefault() : null;
            if (settingForCaching != null)
            {
                //update
                var setting = _settingRepository.Get(settingForCaching.Id);
                setting.Value = valueStr;
            }
            else
            {
                //insert
                var setting = new SettingRecord()
                {
                    Name = key,
                    Value = valueStr
                };

                _settingRepository.Create(setting);
            }


        }
        #endregion
    }
}
