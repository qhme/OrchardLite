using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Orchard.Environment.Configuration;
using Orchard.Mvc.Spooling;
using Orchard.Security;
using Orchard.Security.Permissions;
using Orchard.UI;
using Orchard.UI.Resources;
using Orchard.Localization;

namespace Orchard.Mvc.ViewEngines.Razor
{
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>, IOrchardViewPage
    {
        private ScriptRegister _scriptRegister;
        private ResourceRegister _stylesheetRegister;
        private Localizer _localizer = NullLocalizer.Instance;
        private WorkContext _workContext;
        private IDisplayHelper _displayerHelper;

        public Localizer T
        {
            get
            {
                // first time used, create it
                if (_localizer == NullLocalizer.Instance)
                {
                    _localizer = LocalizationUtilities.Resolve(ViewContext, VirtualPath);
                }

                return _localizer;
            }
        }


        public WorkContext WorkContext { get { return _workContext; } }


        private IAuthorizer _authorizer;
        public IAuthorizer Authorizer
        {
            get
            {
                return _authorizer ?? (_authorizer = _workContext.Resolve<IAuthorizer>());
            }
        }


        public ScriptRegister Script
        {
            get
            {
                return _scriptRegister ??
                    (_scriptRegister = new WebViewScriptRegister(this, Html.ViewDataContainer, ResourceManager));
            }
        }

        private IResourceManager _resourceManager;
        public IResourceManager ResourceManager
        {
            get { return _resourceManager ?? (_resourceManager = _workContext.Resolve<IResourceManager>()); }
        }

        public ResourceRegister Style
        {
            get
            {
                return _stylesheetRegister ??
                    (_stylesheetRegister = new ResourceRegister(Html.ViewDataContainer, ResourceManager, "stylesheet"));
            }
        }

        public void RegisterImageSet(string imageSet, string style = "", int size = 16)
        {
            // hack to fake the style "alternate" for now so we don't have to change stylesheet names when this is hooked up
            // todo: (heskew) deal in shapes so we have real alternates 
            var imageSetStylesheet = !string.IsNullOrWhiteSpace(style)
                ? string.Format("{0}-{1}.css", imageSet, style)
                : string.Format("{0}.css", imageSet);
            Style.Include(imageSetStylesheet);
        }

        public virtual void RegisterLink(LinkEntry link)
        {
            ResourceManager.RegisterLink(link);
        }

        public void SetMeta(string name = null, string content = null, string httpEquiv = null, string charset = null)
        {
            var metaEntry = new MetaEntry();

            if (!String.IsNullOrEmpty(name))
            {
                metaEntry.Name = name;
            }

            if (!String.IsNullOrEmpty(content))
            {
                metaEntry.Content = content;
            }

            if (!String.IsNullOrEmpty(httpEquiv))
            {
                metaEntry.HttpEquiv = httpEquiv;
            }

            if (!String.IsNullOrEmpty(charset))
            {
                metaEntry.Charset = charset;
            }

            SetMeta(metaEntry);
        }

        public virtual void SetMeta(MetaEntry meta)
        {
            ResourceManager.SetMeta(meta);
        }

        public void AppendMeta(string name, string content, string contentSeparator)
        {
            AppendMeta(new MetaEntry { Name = name, Content = content }, contentSeparator);
        }

        public virtual void AppendMeta(MetaEntry meta, string contentSeparator)
        {
            ResourceManager.AppendMeta(meta, contentSeparator);
        }

        public override void InitHelpers()
        {
            base.InitHelpers();

            _workContext = ViewContext.GetWorkContext();

            //Todo:Test
            _displayerHelper = _workContext.Resolve<IDisplayHelper>();

            //_display = DisplayHelperFactory.CreateHelper(ViewContext, this);
        }

        public bool AuthorizedFor(Permission permission)
        {
            return Authorizer.Authorize(permission);
        }

        public bool HasText(object thing)
        {
            return !string.IsNullOrWhiteSpace(Convert.ToString(thing));
        }

        public IDisplayHelper DisplayHelper
        {
            get { return _displayerHelper; }
        }



        //private string _tenantPrefix;
        //public override string Href(string path, params object[] pathParts)
        //{
        //    if (_tenantPrefix == null)
        //    {
        //        _tenantPrefix = WorkContext.Resolve<ShellSettings>().RequestUrlPrefix ?? "";
        //    }

        //    if (!String.IsNullOrEmpty(_tenantPrefix))
        //    {

        //        if (path.StartsWith("~/")
        //            && !path.StartsWith("~/Modules", StringComparison.OrdinalIgnoreCase)
        //            && !path.StartsWith("~/Themes", StringComparison.OrdinalIgnoreCase)
        //            && !path.StartsWith("~/Media", StringComparison.OrdinalIgnoreCase)
        //            && !path.StartsWith("~/Core", StringComparison.OrdinalIgnoreCase))
        //        {

        //            return base.Href("~/" + _tenantPrefix + path.Substring(2), pathParts);
        //        }

        //    }

        //    return base.Href(path, pathParts);
        //}

        //public IDisposable Capture(dynamic zone, string position = null)
        //{
        //    return new CaptureScope(this, html => zone.Add(html, position));
        //}

        public IDisposable Capture(Action<IHtmlString> callback)
        {
            return new CaptureScope(this, callback);
        }



        class CaptureScope : IDisposable
        {
            readonly WebPageBase _viewPage;
            readonly Action<IHtmlString> _callback;

            public CaptureScope(WebPageBase viewPage, Action<IHtmlString> callback)
            {
                _viewPage = viewPage;
                _callback = callback;
                _viewPage.OutputStack.Push(new HtmlStringWriter());
            }

            void IDisposable.Dispose()
            {
                var writer = (HtmlStringWriter)_viewPage.OutputStack.Pop();
                _callback(writer);
            }
        }

        class WebViewScriptRegister : ScriptRegister
        {
            private readonly WebPageBase _viewPage;

            public WebViewScriptRegister(WebPageBase viewPage, IViewDataContainer container, IResourceManager resourceManager)
                : base(container, resourceManager)
            {
                _viewPage = viewPage;
            }

            public override IDisposable Head()
            {
                return new CaptureScope(_viewPage, s => ResourceManager.RegisterHeadScript(s.ToString()));
            }

            public override IDisposable Foot()
            {
                return new CaptureScope(_viewPage, s => ResourceManager.RegisterFootScript(s.ToString()));
            }
        }



    }
    public abstract class WebViewPage : WebViewPage<dynamic>
    {
    }
}
