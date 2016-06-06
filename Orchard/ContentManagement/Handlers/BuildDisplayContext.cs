using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.Handlers
{
    public class BuildDisplayContext : BuildShapeContext
    {
        public BuildDisplayContext(IContent content, string displayType)
            : base(content)
        {
            DisplayType = displayType;
        }

        public string DisplayType { get; private set; }
    }

}
