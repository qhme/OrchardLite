using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Orchard.Core.Settings.Models
{
    public class SettingRecord
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Value { get; set; }
    }
}
