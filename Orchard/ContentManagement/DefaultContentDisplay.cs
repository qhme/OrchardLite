using Orchard.ContentManagement.Handlers;
using Orchard.Environment;
using Orchard.FileSystems.VirtualPath;
using Orchard.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Orchard.ContentManagement
{
    public class DefaultContentDisplay : IContentDisplay
    {
        private readonly Lazy<IEnumerable<IContentHandler>> _handlers;
        private readonly RequestContext _requestContext;
        private readonly IVirtualPathProvider _virtualPathProvider;
        private readonly IWorkContextAccessor _workContextAccessor;

        public DefaultContentDisplay(Lazy<IEnumerable<IContentHandler>> handlers, RequestContext requestContext,
            IVirtualPathProvider virtualPathProvider, IWorkContextAccessor workContextAccessor)
        {
            _handlers = handlers;
            _requestContext = requestContext;
            _virtualPathProvider = virtualPathProvider;
            _workContextAccessor = workContextAccessor;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public string BuildDisplay(IContent content, string displayType = "")
        {
            var contentTypeDefinition = content.ContentItem.TypeDefinition;
            string stereotype;
 
            //var actualShapeType = stereotype;
            //var actualDisplayType = string.IsNullOrWhiteSpace(displayType) ? "Detail" : displayType;

            ////dynamic itemShape = CreateItemShape(actualShapeType);
            ////itemShape.ContentItem = content.ContentItem;
            ////itemShape.Metadata.DisplayType = actualDisplayType;

            //var context = new BuildDisplayContext(content, actualDisplayType);
            //var workContext = _workContextAccessor.GetContext(_requestContext.HttpContext);
            //_handlers.Value.Invoke(handler => handler.BuildDisplay(context), Logger);

            return string.Empty;
         }

        public string BuildEditor(IContent content)
        {
            var contentTypeDefinition = content.ContentItem.TypeDefinition;
            string stereotype;
            //if (!contentTypeDefinition.Settings.TryGetValue("Stereotype", out stereotype))
            //    stereotype = "Content";

            //var actualShapeType = stereotype + "_Edit";

            //dynamic itemShape = CreateItemShape(actualShapeType);
            //itemShape.ContentItem = content.ContentItem;

            // adding an alternate for [Stereotype]_Edit__[ContentType] e.g. Content-Menu.Edit
            //((IShape)itemShape).Metadata.Alternates.Add(actualShapeType + "__" + content.ContentItem.ContentType);

            var context = new BuildEditorContext(content);
            //BindPlacement(context, null, stereotype);

            _handlers.Value.Invoke(handler => handler.BuildEditor(context), Logger);
            return string.Empty;
        }

        public string UpdateEditor(IContent content, IUpdateModel updater)
        {
            var contentTypeDefinition = content.ContentItem.TypeDefinition;
            string stereotype;
            //if (!contentTypeDefinition.Settings.TryGetValue("Stereotype", out stereotype))
            //    stereotype = "Content";

            //var actualShapeType = stereotype + "_Edit";

            //itemShape.ContentItem = content.ContentItem;

            var workContext = _workContextAccessor.GetContext(_requestContext.HttpContext);

            //var theme = workContext.CurrentTheme;
            //var shapeTable = _shapeTableLocator.Value.Lookup(theme.Id);

            // adding an alternate for [Stereotype]_Edit__[ContentType] e.g. Content-Menu.Edit
            //((IShape)itemShape).Metadata.Alternates.Add(actualShapeType + "__" + content.ContentItem.ContentType);

            var context = new UpdateEditorContext(content, updater, GetPath());
            //BindPlacement(context, null, stereotype);
            _handlers.Value.Invoke(handler => handler.UpdateEditor(context), Logger);
            return string.Empty;
        }

        /// <summary>
        /// Gets the current app-relative path, i.e. ~/my-blog/foo.
        /// </summary>
        private string GetPath()
        {
            return VirtualPathUtility.AppendTrailingSlash(_virtualPathProvider.ToAppRelative(_requestContext.HttpContext.Request.Path));
        }
    }
}
