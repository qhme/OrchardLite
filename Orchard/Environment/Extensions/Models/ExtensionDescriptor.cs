using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Environment.Extensions.Models
{
    public class ExtensionDescriptor
    {
        /// <summary>
        /// Virtual path base, "~/Modules", or "~/Core"
        /// </summary>
        public string Location { get; set; }

        public string Id { get; set; }

        /// <summary>
        /// The extension type.
        /// </summary>
        public string ExtensionType { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string Description { get; set; }

        public string SessionState { get; set; }

        public IEnumerable<FeatureDescriptor> Features { get; set; }
    }
}
