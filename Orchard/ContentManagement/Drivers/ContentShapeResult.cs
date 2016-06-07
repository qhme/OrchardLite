using Orchard.ContentManagement.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.Drivers
{
    public class ContentShapeResult : DriverResult
    {
        private string _defaultLocation;
        private string _differentiator;
        private readonly string _shapeType;
        private readonly string _prefix;
        private readonly Func<BuildShapeContext, dynamic> _shapeBuilder;
        private string _groupId;

        public ContentShapeResult(string shapeType, string prefix, Func<BuildShapeContext, dynamic> shapeBuilder)
        {
            _shapeType = shapeType;
            _prefix = prefix;
            _shapeBuilder = shapeBuilder;
        }

        public override void Apply(BuildDisplayContext context)
        {
            ApplyImplementation(context, context.DisplayType);
        }

        public override void Apply(BuildEditorContext context)
        {
            ApplyImplementation(context, null);
        }

        private void ApplyImplementation(BuildShapeContext context, string displayType)
        {
            //dynamic parentShape = context.Shape;
            context.ContentPart = ContentPart;

            var newShape = _shapeBuilder(context);

            // ignore it if the driver returned a null shape
            if (newShape == null)
            {
                return;
            }

            // add a ContentPart property to the final shape 
            if (ContentPart != null && newShape.ContentPart == null)
            {
                newShape.ContentPart = ContentPart;
            }



            //ShapeMetadata newShapeMetadata = newShape.Metadata;
            //newShapeMetadata.Prefix = _prefix;
            //newShapeMetadata.DisplayType = displayType;


            //if (String.IsNullOrEmpty(position))
            //{
            //    parentShape.Zones[zone].Add(newShape);
            //}
            //else
            //{
            //    parentShape.Zones[zone].Add(newShape, position);
            //}
        }

        public ContentShapeResult Location(string zone)
        {
            _defaultLocation = zone;
            return this;
        }

        public ContentShapeResult Differentiator(string differentiator)
        {
            _differentiator = differentiator;
            return this;
        }

        public ContentShapeResult OnGroup(string groupId)
        {
            _groupId = groupId;
            return this;
        }

        public string GetDifferentiator()
        {
            return _differentiator;
        }

        public string GetGroup()
        {
            return _groupId;
        }

        public string GetLocation()
        {
            return _defaultLocation;
        }

        public string GetShapeType()
        {
            return _shapeType;
        }
    }

}
