using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Environment
{
    public interface IWorkContextEvents : ISingletonDependency
    {
        void Started();

        /// <summary>
        /// Fired when the work context is finished.
        /// </summary>
        void Finished();

    }
}
