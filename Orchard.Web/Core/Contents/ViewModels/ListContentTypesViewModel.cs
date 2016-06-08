using System.Collections.Generic;

namespace Orchard.Core.Contents.ViewModels
{
    public class ListContentTypesViewModel
    {
        public IEnumerable<EditTypeViewModel> Types { get; set; }
    }
}