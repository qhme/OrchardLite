using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Tasks
{
    public interface IBackgroundTask : IDependency
    {
        void Sweep();
    }
}
