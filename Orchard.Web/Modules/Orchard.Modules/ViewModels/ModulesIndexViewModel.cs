using System.Collections.Generic;
using Orchard.Modules.Models;
using Orchard.UI.Navigation;

namespace Orchard.Modules.ViewModels
{
    public class ModulesIndexViewModel
    {
        public bool InstallModules { get; set; }
        public IEnumerable<ModuleEntry> Modules { get; set; }

        public ModulesIndexOptions Options { get; set; }
        public Pager Pager { get; set; }
    }

    public class ModulesIndexOptions
    {
        public string SearchText { get; set; }
    }
}