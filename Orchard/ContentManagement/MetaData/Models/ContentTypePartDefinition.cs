using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.MetaData.Models
{
    public class ContentTypePartDefinition
    {
        public ContentTypePartDefinition(String partName)
        {
            PartName = partName;
        }

        public ContentTypePartDefinition()
        {
        }

        public String PartName { get; private set; }

        public int Index { get; set; }

    }

}
