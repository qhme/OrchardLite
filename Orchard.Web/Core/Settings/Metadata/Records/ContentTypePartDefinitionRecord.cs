using Orchard.Data.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.Core.Settings.Metadata.Records
{
    public class ContentTypePartDefinitionRecord
    {
        public virtual int Id { get; set; }

        public string TypeName { get; set; }

        public string PartName { get; set; }

        public int Ord { get; set; }
    }
}