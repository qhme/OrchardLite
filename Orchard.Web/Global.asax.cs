using Autofac;
using StackExchange.Profiling;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using Orchard;
using Orchard.Data;
using Orchard.Environment;
using Orchard.Infrastructure;
using Orchard.Logging;
using Orchard.WarmupStarter;

namespace Orchard.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static Starter<IOrchardHost> _starter;

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //register custom routes (plugins, etc)
            //var routePublisher = EngineContext.Current.Resolve<IRoutePublisher>();
            //routePublisher.RegisterRoutes(routes);

            //routes.MapRoute(
            //    "Default", // Route name
            //    "{controller}/{action}/{id}", // URL with parameters
            //    new { controller = "Home", action = "Index", id = UrlParameter.Optional },
            //    new[] { "Orchard.Web.Controllers" }
            //);
        }

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);

            _starter = new Starter<IOrchardHost>(HostInitialization, HostBeginRequest, HostEndRequest);
            _starter.OnApplicationStart(this);

            //fluent validation
            //DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            //ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new NopValidatorFactory()));

            ////register virtual path provider for embedded views
            //var embeddedViewResolver = EngineContext.Current.Resolve<IEmbeddedViewResolver>();
            //var embeddedProvider = new EmbeddedViewVirtualPathProvider(embeddedViewResolver.GetEmbeddedViews());
            //HostingEnvironment.RegisterVirtualPathProvider(embeddedProvider);

            //start scheduled tasks
            //if (databaseInstalled)
            //{
            //    TaskManager.Instance.Initialize();
            //    TaskManager.Instance.Start();
            //}
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //ignore static resources
            //var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            //if (webHelper.IsStaticResource(this.Request))
            //    return;

            //EnsureDatabaseIsInstalled();

            //if (CanPerformProfilingAction())
            //{
            //    MiniProfiler.Start();
            //}

            _starter.OnBeginRequest(this);
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            _starter.OnEndRequest(this);

            //if (CanPerformProfilingAction())
            //{
            //    //stop as early as you can, even earlier with MvcMiniProfiler.MiniProfiler.Stop(discardResults: true);
            //    MiniProfiler.Stop();
            //}
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
        }


        protected bool CanPerformProfilingAction()
        {
            //will not run in medium trust
            if (CommonHelper.GetTrustLevel() < AspNetHostingPermissionLevel.High)
                return false;

            //if (!DataSettingsHelper.DatabaseIsInstalled())
            //    return false;

            return true;
            //var workContext = EngineContext.Current.Resolve<IWorkContext>();
            //return workContext.SiteSettings.DisplayMiniProfiler;
        }

        private static void HostBeginRequest(HttpApplication application, IOrchardHost host)
        {
            application.Context.Items["originalHttpContext"] = application.Context;
            host.BeginRequest();
        }

        private static void HostEndRequest(HttpApplication application, IOrchardHost host)
        {
            host.EndRequest();
        }

        private static IOrchardHost HostInitialization(HttpApplication application)
        {
            var host = OrchardStarter.CreateHost(MvcSingletons);

            host.Initialize();

            // initialize shells to speed up the first dynamic query
            host.BeginRequest();
            host.EndRequest();

            return host;
        }

        static void MvcSingletons(ContainerBuilder builder)
        {
            builder.Register(ctx => RouteTable.Routes).SingleInstance();
            builder.Register(ctx => ModelBinders.Binders).SingleInstance();
            builder.Register(ctx => ViewEngines.Engines).SingleInstance();
        }
    }

}
