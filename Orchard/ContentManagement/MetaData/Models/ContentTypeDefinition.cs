using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Utility.Extensions;

namespace Orchard.ContentManagement.MetaData.Models
{
    public class ContentTypeDefinition
    {
        public ContentTypeDefinition(string name, IEnumerable<ContentTypePartDefinition> parts)
        {
            Name = name;
            Parts = parts.ToReadOnlyCollection();
        }

        public ContentTypeDefinition(string name)
        {
            Name = name;
            Parts = Enumerable.Empty<ContentTypePartDefinition>();
        }

        [StringLength(128)]
        public string Name { get; private set; }

        public string Description { get; set; }
        public IEnumerable<ContentTypePartDefinition> Parts { get; private set; }
    }
}
