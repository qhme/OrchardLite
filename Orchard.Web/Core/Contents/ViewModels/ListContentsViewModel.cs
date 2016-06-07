using Orchard.ContentManagement;
using Orchard.UI.Navigation;
using System.Collections.Generic;

namespace Orchard.Core.Contents.ViewModels
{
    public class ListContentsViewModel
    {
        public Pager Pager { get; set; }

        public List<ContentItem> Items { get; set; }
    }
}