using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.MetaData.Models
{
    public class ContentTypePartDefinition
    {
        public ContentTypePartDefinition(ContentPartDefinition contentPartDefinition, SettingsDictionary settings)
        {
            PartDefinition = contentPartDefinition;
            Settings = settings;
        }

        public ContentTypePartDefinition()
        {
            Settings = new SettingsDictionary();
        }

        public ContentPartDefinition PartDefinition { get; private set; }
        public SettingsDictionary Settings { get; private set; }
        public ContentTypeDefinition ContentTypeDefinition { get; set; }
    }

}
