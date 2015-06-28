using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Environment
{
    public interface IOrchardHostContainer
    {
        T Resolve<T>();
    }
}
