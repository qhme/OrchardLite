using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.Handlers
{
    public interface IContentActivatingFilter : IContentFilter
    {
        void Activating(ActivatingContentContext context);
    }
}
