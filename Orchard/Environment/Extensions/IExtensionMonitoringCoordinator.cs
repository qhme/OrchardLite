using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Caching;

namespace Orchard.Environment.Extensions
{
    public interface IExtensionMonitoringCoordinator
    {
        void MonitorExtensions(Action<IVolatileToken> monitor);
    }
}
