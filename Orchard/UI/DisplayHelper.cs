using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Orchard.Environment;
using Orchard.Mvc;
using Orchard.UI.Resources;

namespace Orchard.UI
{
    public interface IDisplayHelper : IDependency
    {
        void HeadScripts(TextWriter output);

        void FootScripts(TextWriter output);

        void Metas(TextWriter output);

        void HeadLinks(TextWriter output);

        void StylesheetLinks(TextWriter Output);

        IHtmlString Pager_Links(HtmlHelper Html,
            int Page,
            int PageSize,
            double TotalItemCount,
            int? Quantity,
            object FirstText,
            object PreviousText,
            object NextText,
            object LastText,
            object GapText,
            string PagerId);

    }

    public class DisplayHelper : IDisplayHelper
    {
        private readonly Work<WorkContext> _workContext;
        private readonly Work<IResourceManager> _resourceManager;
        private readonly Work<IHttpContextAccessor> _httpContextAccessor;

        public DisplayHelper(Work<WorkContext> workContext, Work<IResourceManager> resourceManager, Work<IHttpContextAccessor> httpContextAccessor)
        {
            _workContext = workContext;
            _resourceManager = resourceManager;
            _httpContextAccessor = httpContextAccessor;
        }


        public void HeadScripts(TextWriter output)
        {
            WriteResources(output, "script", ResourceLocation.Head, null);
            WriteLiteralScripts(output, _resourceManager.Value.GetRegisteredHeadScripts());
        }

        public void FootScripts(TextWriter output)
        {
            WriteResources(output, "script", null, ResourceLocation.Head);
            WriteLiteralScripts(output, _resourceManager.Value.GetRegisteredFootScripts());
        }

        public void Metas(TextWriter output)
        {
            foreach (var meta in _resourceManager.Value.GetRegisteredMetas())
            {
                output.WriteLine(meta.GetTag());
            }
        }

        public void HeadLinks(TextWriter output)
        {
            foreach (var link in _resourceManager.Value.GetRegisteredLinks())
            {
                output.WriteLine(link.GetTag());
            }
        }

        public void StylesheetLinks(TextWriter Output)
        {
            WriteResources(Output, "stylesheet", null, null);
        }

        public void Style(HtmlHelper Html, TextWriter Output, ResourceDefinition Resource, string Url, string Condition, Dictionary<string, string> TagAttributes)
        {
            // do not write to Output directly as Styles are rendered in Zones
            ResourceManager.WriteResource(Html.ViewContext.Writer, Resource, Url, Condition, TagAttributes);
        }

        public void Script(HtmlHelper Html, TextWriter Output, ResourceDefinition Resource, string Url, string Condition, Dictionary<string, string> TagAttributes)
        {
            // do not write to Output directly as Styles are rendered in Zones
            ResourceManager.WriteResource(Html.ViewContext.Writer, Resource, Url, Condition, TagAttributes);
        }

        public void Resource(TextWriter Output, ResourceDefinition Resource, string Url, string Condition, Dictionary<string, string> TagAttributes)
        {
            ResourceManager.WriteResource(Output, Resource, Url, Condition, TagAttributes);
        }


        public IHtmlString Pager_Links(HtmlHelper Html, int Page, int PageSize, double TotalItemCount, int? Quantity, object FirstText, object PreviousText, object NextText, object LastText, object GapText, string PagerId)
        {
            throw new NotImplementedException();
        }

        private static void WriteLiteralScripts(TextWriter output, IEnumerable<string> scripts)
        {
            if (scripts == null)
            {
                return;
            }
            foreach (string script in scripts)
            {
                output.WriteLine(script);
            }
        }

        private void WriteResources(TextWriter Output, string resourceType, ResourceLocation? includeLocation, ResourceLocation? excludeLocation)
        {
            var defaultSettings = new RequireSettings();

            var requiredResources = _resourceManager.Value.BuildRequiredResources(resourceType);
            var appPath = _httpContextAccessor.Value.Current().Request.ApplicationPath;
            foreach (var context in requiredResources.Where(r =>
                (includeLocation.HasValue ? r.Settings.Location == includeLocation.Value : true) &&
                (excludeLocation.HasValue ? r.Settings.Location != excludeLocation.Value : true)))
            {

                var path = context.GetResourceUrl(defaultSettings, appPath);
                var condition = context.Settings.Condition;
                var attributes = context.Settings.HasAttributes ? context.Settings.Attributes : null;

                //需要判断是否用了cdn地址
                ResourceManager.WriteResource(Output, context.Resource, !context.Resource.Url.StartsWith("http") ? (context.Resource.BasePath + context.Resource.Url) : context.Resource.Url,
                    condition, attributes);
                //if (resourceType == "stylesheet")
                //{
                //    context.Resource.TagBuilder.Attributes.Add(context.Resource.FilePathAttributeName, context.Resource.Url);

                //}
                //else if (resourceType == "script")
                //{
                //    result = Display.Script(Url: path, Condition: condition, Resource: context.Resource, TagAttributes: attributes);
                //}
                //else
                //{
                //    result = Display.Resource(Url: path, Condition: condition, Resource: context.Resource, TagAttributes: attributes);
                //}
                //Output.Write(result);
            }
        }

    }
}
