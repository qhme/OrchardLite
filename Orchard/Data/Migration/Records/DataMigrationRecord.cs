using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement.Records;

namespace Orchard.Data.Migration.Records
{
    public class DataMigrationRecord : ContentItem
    {
        public virtual string DataMigrationClass { get; set; }

        public virtual int? Version { get; set; }
    }
}
