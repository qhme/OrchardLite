using Orchard.Data.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.Records
{
    public class ContentItemRecord
    {
        public virtual int Id { get; set; }

        public virtual string ContentType { get; set; }
  
    }
}
