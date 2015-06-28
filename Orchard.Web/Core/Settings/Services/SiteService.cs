using System.Linq;
using Orchard.Caching;
using Orchard.Core.Settings.Models;
using Orchard.ContentManagement;
using Orchard.Settings;

namespace Orchard.Core.Settings.Services
{
    public class SiteService : ISiteService
    {
        private readonly ISettingService _settingService;

        public SiteService(ISettingService settingService)
        {
            _settingService = settingService;
        }

        public ISite GetSiteSettings()
        {
            var siteSettings = _settingService.LoadSetting<SiteSettings>();
            if (siteSettings.PageSize == 0)
                siteSettings.PageSize = 10;
            return siteSettings;
        }
    }
}