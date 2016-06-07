using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.Handlers
{
    public interface IContentStorageFilter : IContentFilter
    {
        void Activated(ActivatedContentContext context);
        void Initializing(InitializingContentContext context);
        void Initialized(InitializingContentContext context);

        void Creating(CreateContentContext context);
        void Created(CreateContentContext context);

        void Updating(UpdateContentContext context);

        void Updated(UpdateContentContext context);

        void Removing(RemoveContentContext context);

        void Removed(RemoveContentContext context);
        void Destroying(DestroyContentContext context);
        void Destroyed(DestroyContentContext context);
    }

}
