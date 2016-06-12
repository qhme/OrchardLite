using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.Content2.Models2
{
    public class C2PartRecord : ContentPartRecord
    {
        public virtual string Text { get { return "Hello world from C2Part!"; } }
       
    }
}