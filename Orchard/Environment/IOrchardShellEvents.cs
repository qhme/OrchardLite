using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Events;

namespace Orchard.Environment
{
    public interface IOrchardShellEvents : IEventHandler
    {
        void Activated();

        void Terminating();
    }
}
