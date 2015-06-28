using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Logging;
using System.Threading.Tasks;
using Orchard.FileSystems.AppData;
using System.IO;

namespace Orchard.Environment.Configuration
{
    public class ShellSettingsManager : Component, IShellSettingsManager
    {
        private const string _settingsFileName = "Settings.txt";
        private readonly IAppDataFolder _appDataFolder;
        private readonly IShellSettingsManagerEventHandler _events;


        public ShellSettingsManager(IAppDataFolder appDataFolder, IShellSettingsManagerEventHandler events)
        {
            _appDataFolder = appDataFolder;
            _events = events;
        }

        ShellSettings IShellSettingsManager.LoadSettings()
        {
            Logger.Debug("Reading ShellSettings...");
            var settings = LoadSettingsInternal();

            Logger.Debug("Returning ShellSettings objects.");

            return settings;
        }

        void IShellSettingsManager.SaveSettings(ShellSettings settings)
        {
            if (settings == null)
            {
                Logger.Error("shell settings is null");
                throw new ArgumentNullException("settings");
            }

            Logger.Debug("Saving ShellSettings");
            _appDataFolder.CreateFile(_settingsFileName, ShellSettingsSerializer.ComposeSettings(settings));

            Logger.Information("ShellSettings saved successfully; ");
            _events.Saved(settings);

        }

        private ShellSettings LoadSettingsInternal()
        {
            if (!_appDataFolder.FileExists(_settingsFileName))
                return null;


            return ShellSettingsSerializer.ParseSettings(_appDataFolder.ReadFile(_settingsFileName));
        }
    }


}
