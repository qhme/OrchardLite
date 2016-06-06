using Orchard.ContentManagement.Records;
using Orchard.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.Handlers
{
    public class BuildShapeContext
    {
        protected BuildShapeContext(IContent content)
        {
            Content = content;

        }

        public IContent Content { get; private set; }
        public ContentItem ContentItem { get; private set; }
        public string GroupId { get; private set; }
        public ContentPart ContentPart { get; set; }
        public ILogger Logger { get; set; }


    }

}
