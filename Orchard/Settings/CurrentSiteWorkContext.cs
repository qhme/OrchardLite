using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Environment;

namespace Orchard.Settings
{
    public class CurrentSiteWorkContext : IWorkContextStateProvider
    {
        private readonly ISiteService _siteService;

        public CurrentSiteWorkContext(ISiteService siteService)
        {
            _siteService = siteService;
        }

        public Func<WorkContext, T> Get<T>(string name)
        {
            if (name == "CurrentSite")
            {
                var siteSettings = _siteService.GetSiteSettings();
                return ctx => (T)siteSettings;
            }
            return null;
        }
    }

}
