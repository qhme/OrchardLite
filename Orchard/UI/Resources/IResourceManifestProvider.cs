using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.UI.Resources
{
    public interface IResourceManifestProvider : ISingletonDependency
    {
        void BuildManifests(ResourceManifestBuilder builder);
    }
}
