using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.Handlers
{
    public interface IContentHandler : IDependency
    {
        void Creating(CreateContentContext context);
        void Created(CreateContentContext context);
        void Updating(UpdateContentContext context);
        void Updated(UpdateContentContext context);
        void Removing(RemoveContentContext context);
        void Removed(RemoveContentContext context);
    }
}
