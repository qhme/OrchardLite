using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Environment
{
    public interface IOrchardShell
    {
        void Activate();
        void Terminate();
    }
}
