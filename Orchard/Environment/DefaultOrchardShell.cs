using Autofac.Features.OwnedInstances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Environment.Configuration;
using Orchard.Logging;
using Orchard.Mvc.ModelBinders;
using Orchard.Mvc.Routes;
using Orchard.Tasks;
using Orchard.WebApi.Routes;

namespace Orchard.Environment
{
    public class DefaultOrchardShell : IOrchardShell
    {
        private readonly Func<Owned<IOrchardShellEvents>> _eventsFactory;
        private readonly IEnumerable<IRouteProvider> _routeProviders;
        private readonly IEnumerable<IHttpRouteProvider> _httpRouteProviders;
        private readonly IRoutePublisher _routePublisher;
        private readonly IEnumerable<IModelBinderProvider> _modelBinderProviders;
        private readonly IModelBinderPublisher _modelBinderPublisher;
        private readonly ISweepGenerator _sweepGenerator;
        private readonly ShellSettings _shellSettings;

        public DefaultOrchardShell(
             Func<Owned<IOrchardShellEvents>> eventsFactory,
             IEnumerable<IRouteProvider> routeProviders,
             IEnumerable<IHttpRouteProvider> httpRouteProviders,
             IRoutePublisher routePublisher,
             IEnumerable<IModelBinderProvider> modelBinderProviders,
             IModelBinderPublisher modelBinderPublisher,
             ISweepGenerator sweepGenerator,
            ShellSettings shellSettings)
        {
            _eventsFactory = eventsFactory;
            _routeProviders = routeProviders;
            _routePublisher = routePublisher;
            _modelBinderProviders = modelBinderProviders;
            _modelBinderPublisher = modelBinderPublisher;
            _sweepGenerator = sweepGenerator;
            _httpRouteProviders = httpRouteProviders;
            _shellSettings = shellSettings;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void Activate()
        {
            //Owin

            var allRoutes = new List<RouteDescriptor>();
            _routeProviders.Invoke(x => x.GetRoutes(allRoutes), Logger);
            allRoutes.AddRange(_httpRouteProviders.SelectMany(provider => provider.GetRoutes()));

            _routePublisher.Publish(allRoutes);
            _modelBinderPublisher.Publish(_modelBinderProviders.SelectMany(provider => provider.GetModelBinders()));

            using (var events = _eventsFactory())
            {
                events.Value.Activated();
            }

            _sweepGenerator.Activate();
        }

        public void Terminate()
        {
            SafelyTerminate(() =>
            {
                using (var events = _eventsFactory())
                {
                    SafelyTerminate(() => events.Value.Terminating());
                }
            });

            SafelyTerminate(() => _sweepGenerator.Terminate());
        }

        private void SafelyTerminate(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Logger.Error(e, "An unexcepted error occured while terminating the shell");
            }
        }
    }
}
