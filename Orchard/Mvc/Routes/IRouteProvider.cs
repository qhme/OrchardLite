using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Mvc.Routes
{
    public interface IRouteProvider : IDependency
    {
        void GetRoutes(ICollection<RouteDescriptor> routes);
    }
}
