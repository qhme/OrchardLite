using System.Collections.Generic;
using Orchard.Alias.Implementation.Holder;
using Orchard.UI.Navigation;

namespace Orchard.Alias.ViewModels
{

    public class AdminIndexViewModel
    {
        public IList<AliasEntry> AliasEntries { get; set; }
        public AdminIndexOptions Options { get; set; }
        public Pager Pager { get; set; }
    }

    public class AliasEntry
    {
        public AliasInfo Alias { get; set; }
        public bool IsChecked { get; set; }
    }
    public class AdminIndexOptions
    {
        public string Search { get; set; }
        public AliasOrder Order { get; set; }
        public AliasFilter Filter { get; set; }
        public AliasBulkAction BulkAction { get; set; }
    }

    public enum AliasOrder
    {
        Path
    }

    public enum AliasFilter
    {
        All
    }

    public enum AliasBulkAction
    {
        None,
        Delete
    }
}