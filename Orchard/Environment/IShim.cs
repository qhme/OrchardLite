using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Environment
{
    /// <summary>
    /// Interface implemented by shims for ASP.NET singleton services that
    /// need access to the HostContainer instance.
    /// </summary>
    public interface IShim
    {
        IOrchardHostContainer HostContainer { get; set; }
    }
}
