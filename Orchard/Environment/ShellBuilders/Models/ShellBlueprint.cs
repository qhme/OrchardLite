using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Data;
using Orchard.Environment.Configuration;
using Orchard.Environment.Descriptor.Models;
using Orchard.Environment.Extensions.Models;

namespace Orchard.Environment.ShellBuilders.Models
{
    public class ShellBlueprint
    {
        public ShellSettings Settings { get; set; }

        public ShellDescriptor Descriptor { get; set; }
        public IEnumerable<DependencyBlueprint> Dependencies { get; set; }
        public IEnumerable<ControllerBlueprint> Controllers { get; set; }
        public IEnumerable<ControllerBlueprint> HttpControllers { get; set; }
        public IEnumerable<RecordBlueprint> Records { get; set; }

    }

    public class ShellBlueprintItem
    {
        public Type Type { get; set; }
        public Feature Feature { get; set; }
    }

    public class DependencyBlueprint : ShellBlueprintItem
    {
        public IEnumerable<ShellParameter> Parameters { get; set; }
    }

    public class ControllerBlueprint : ShellBlueprintItem
    {
        public string AreaName { get; set; }
        public string ControllerName { get; set; }
    }

    public class RecordBlueprint : ShellBlueprintItem
    {
        public string TableName { get; set; }
    }

}
