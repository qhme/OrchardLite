using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.Core.Contents.ViewModels
{
    public class ListContentPartsViewModel
    {
        public IEnumerable<EditPartViewModel> Parts { get; set; }
    }
}