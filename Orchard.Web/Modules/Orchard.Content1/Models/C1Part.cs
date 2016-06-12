using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.Content1.Models2
{
    public class C1Part : ContentPart<C1PartRecord>
    {
        public string Text { get { return Record.Text; } }
    }
}