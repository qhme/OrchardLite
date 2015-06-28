using System.Web.Routing;
using System.Web.SessionState;

namespace Orchard.Mvc.Routes
{
    public class RouteDescriptor
    {
        public string Name { get; set; }
        public int Priority { get; set; }
        public RouteBase Route { get; set; }
        public SessionStateBehavior SessionState { get; set; }
    }
}
