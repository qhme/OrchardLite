using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Mvc.Extensions;

namespace Orchard.Mvc.Routes
{
    public class ShellRoute : RouteBase, IRouteWithArea
    {
        private readonly RouteBase _route;
        private readonly ShellSettings _shellSettings;
        private readonly IWorkContextAccessor _workContextAccessor;

        public ShellRoute(RouteBase route, ShellSettings shellSettings, IWorkContextAccessor workContextAccessor)
        {
            _route = route;
            _shellSettings = shellSettings;
            _workContextAccessor = workContextAccessor;

            Area = route.GetAreaName();
        }

        public SessionStateBehavior SessionState { get; set; }

        public string Area { get; private set; }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var effectiveHttpContext = httpContext;

            var routeData = _route.GetRouteData(effectiveHttpContext);
            if (routeData == null)
                return null;

            // otherwise wrap handler and return it
            routeData.RouteHandler = new RouteHandler(_workContextAccessor, routeData.RouteHandler, SessionState);
            routeData.DataTokens["IWorkContextAccessor"] = _workContextAccessor;

            return routeData;
        }


        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            var effectiveRequestContext = requestContext;
            var virtualPath = _route.GetVirtualPath(effectiveRequestContext, values);

            return virtualPath;
        }

        class RouteHandler : IRouteHandler
        {
            private readonly IWorkContextAccessor _workContextAccessor;
            private readonly IRouteHandler _routeHandler;
            private readonly SessionStateBehavior _sessionStateBehavior;

            public RouteHandler(IWorkContextAccessor workContextAccessor, IRouteHandler routeHandler, SessionStateBehavior sessionStateBehavior)
            {
                _workContextAccessor = workContextAccessor;
                _routeHandler = routeHandler;
                _sessionStateBehavior = sessionStateBehavior;
            }

            public IHttpHandler GetHttpHandler(RequestContext requestContext)
            {
                var httpHandler = _routeHandler.GetHttpHandler(requestContext);
                requestContext.HttpContext.SetSessionStateBehavior(_sessionStateBehavior);

                if (httpHandler is IHttpAsyncHandler)
                {
                    return new HttpAsyncHandler(_workContextAccessor, (IHttpAsyncHandler)httpHandler);
                }
                return new HttpHandler(_workContextAccessor, httpHandler);
            }
        }

        class HttpHandler : IHttpHandler, IRequiresSessionState, IHasRequestContext
        {
            protected readonly IWorkContextAccessor _workContextAccessor;
            private readonly IHttpHandler _httpHandler;

            public HttpHandler(IWorkContextAccessor workContextAccessor, IHttpHandler httpHandler)
            {
                _workContextAccessor = workContextAccessor;
                _httpHandler = httpHandler;
            }

            public bool IsReusable
            {
                get { return false; }
            }

            public void ProcessRequest(HttpContext context)
            {
                using (_workContextAccessor.CreateWorkContextScope(new HttpContextWrapper(context)))
                {
                    _httpHandler.ProcessRequest(context);
                }
            }

            public RequestContext RequestContext
            {
                get
                {
                    var mvcHandler = _httpHandler as MvcHandler;
                    return mvcHandler == null ? null : mvcHandler.RequestContext;
                }
            }
        }

        class HttpAsyncHandler : HttpHandler, IHttpAsyncHandler
        {
            private readonly IHttpAsyncHandler _httpAsyncHandler;
            private IDisposable _scope;

            public HttpAsyncHandler(IWorkContextAccessor containerProvider, IHttpAsyncHandler httpAsyncHandler)
                : base(containerProvider, httpAsyncHandler)
            {
                _httpAsyncHandler = httpAsyncHandler;
            }

            public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
            {
                _scope = _workContextAccessor.CreateWorkContextScope(new HttpContextWrapper(context));
                try
                {
                    return _httpAsyncHandler.BeginProcessRequest(context, cb, extraData);
                }
                catch
                {
                    _scope.Dispose();
                    throw;
                }
            }

            [DebuggerStepThrough]
            public void EndProcessRequest(IAsyncResult result)
            {
                try
                {
                    _httpAsyncHandler.EndProcessRequest(result);
                }
                finally
                {
                    _scope.Dispose();
                }
            }
        }
    }


}
