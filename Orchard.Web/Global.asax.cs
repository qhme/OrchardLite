using Autofac;
//using StackExchange.Profiling;
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
        }

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);

            _starter = new Starter<IOrchardHost>(HostInitialization, HostBeginRequest, HostEndRequest);
            _starter.OnApplicationStart(this);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            _starter.OnBeginRequest(this);
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            _starter.OnEndRequest(this);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
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
