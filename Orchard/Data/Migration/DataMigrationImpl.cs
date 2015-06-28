using Orchard.Data.Migration.Schema;
using Orchard.Environment.Extensions.Models;

namespace Orchard.Data.Migration
{
    public abstract class DataMigrationImpl : IDataMigration
    {
        public virtual SchemaBuilder SchemaBuilder { get; set; }

        public virtual Feature Feature { get; set; }
    }
}
