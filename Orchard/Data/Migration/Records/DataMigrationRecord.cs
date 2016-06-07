using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement;

namespace Orchard.Data.Migration.Records
{
    public class DataMigrationRecord
    {
        public virtual int Id { get; set; }

        public virtual string DataMigrationClass { get; set; }

        public virtual int? Version { get; set; }
    }
}
