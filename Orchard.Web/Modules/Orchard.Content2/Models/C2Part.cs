using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.Content2.Models2
{
    public class C2Part : ContentPart<C2PartRecord>
    {
        public string Text { get { return Record.Text; } }
    }
}