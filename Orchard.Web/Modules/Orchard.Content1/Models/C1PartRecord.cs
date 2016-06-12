using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.Content1.Models2
{
    public class C1PartRecord : ContentPartRecord
    {
        public virtual string Text { get { return "Hello world from C1Part!"; } }
    }
}