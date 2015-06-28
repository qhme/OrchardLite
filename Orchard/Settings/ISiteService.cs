using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Settings
{
    public interface ISiteService : IDependency
    {
        ISite GetSiteSettings();
    }
}
