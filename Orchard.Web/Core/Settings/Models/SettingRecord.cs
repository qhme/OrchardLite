using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement.Records;

namespace Orchard.Core.Settings.Models
{
    public class SettingRecord : ContentItem
    {
        public virtual string Name { get; set; }

        public virtual string Value { get; set; }
    }
}
