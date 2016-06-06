using Orchard.ContentManagement.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.Drivers
{
    public abstract class ContentPartDriver<TContent> : IContentPartDriver where TContent : ContentPart, new()
    {
        protected virtual string Prefix { get { return typeof(TContent).Name; } }

        public DriverResult BuildDisplay(Handlers.BuildDisplayContext context)
        {
            var part = context.ContentItem.As<TContent>();

            if (part == null)
            {
                return null;
            }

            DriverResult result = Display(part, context.DisplayType, context);

            if (result != null)
            {
                result.ContentPart = part;
            }

            return result;
        }

        public DriverResult BuildEditor(Handlers.BuildEditorContext context)
        {
            var part = context.ContentItem.As<TContent>();

            if (part == null)
            {
                return null;
            }

            DriverResult result = Editor(part, context);

            if (result != null)
            {
                result.ContentPart = part;
            }

            return result;
        }

        public DriverResult UpdateEditor(Handlers.UpdateEditorContext context)
        {
            var part = context.ContentItem.As<TContent>();

            if (part == null)
            {
                return null;
            }

            // checking if the editor needs to be updated (e.g. if it was not hidden)
            var editor = Editor(part, context) as ContentShapeResult;

            if (editor != null)
            {
                ShapeDescriptor descriptor;
                if (context.ShapeTable.Descriptors.TryGetValue(editor.GetShapeType(), out descriptor))
                {
                    var placementContext = new ShapePlacementContext
                    {
                        Content = part.ContentItem,
                        ContentType = part.ContentItem.ContentType,
                        Differentiator = editor.GetDifferentiator(),
                        DisplayType = null,
                        Path = context.Path
                    };

                    var location = descriptor.Placement(placementContext).Location;

                    if (String.IsNullOrEmpty(location) || location == "-")
                    {
                        return editor;
                    }

                    var editorGroup = editor.GetGroup() ?? "";
                    var contextGroup = context.GroupId ?? "";

                    if (!String.Equals(editorGroup, contextGroup, StringComparison.OrdinalIgnoreCase))
                    {
                        return editor;
                    }
                }
            }

            DriverResult result = Editor(part, context.Updater, context.New);

            if (result != null)
            {
                result.ContentPart = part;
            }

            return result;
        }

        public IEnumerable<MetaData.ContentPartInfo> GetPartInfo()
        {
            var contentPartInfo = new[] {
                new ContentPartInfo {
                    PartName = typeof (TContent).Name,
                    Factory = typePartDefinition => new TContent {TypePartDefinition = typePartDefinition}
                }
            };

            return contentPartInfo;
        }
    }
}
