using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.MetaData.Models
{
    public class ContentTypePartDefinition
    {
        public ContentTypePartDefinition(String partName, SettingsDictionary settings)
        {
            PartName = partName;
            Settings = settings;
        }

        public ContentTypePartDefinition()
        {
            Settings = new SettingsDictionary();
        }

        public String PartName { get; private set; }
        public SettingsDictionary Settings { get; private set; }
        public ContentTypeDefinition ContentTypeDefinition { get; set; }
    }

}
