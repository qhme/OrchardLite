using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.MetaData.Models
{
    public class ContentPartDefinition
    {
        public ContentPartDefinition(string name, SettingsDictionary settings)
        {
            Name = name;
            Settings = settings;
        }

        public ContentPartDefinition(string name)
        {
            Name = name;
            Settings = new SettingsDictionary();
        }

        public string Name { get; private set; }
        
        public SettingsDictionary Settings { get; private set; }
    }
}
