using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.UI.Resources
{
    public interface IResourceManifest
    {
        ResourceDefinition DefineResource(string resourceType, string resourceName);
        string Name { get; }
        string BasePath { get; }
        IDictionary<string, ResourceDefinition> GetResources(string resourceType);
    }

}
