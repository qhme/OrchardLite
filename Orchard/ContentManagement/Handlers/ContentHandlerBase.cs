using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.Handlers
{
    public class ContentHandlerBase : IContentHandler
    {
        public virtual void Creating(CreateContentContext context)
        {
        }

        public virtual void Created(CreateContentContext context)
        {
        }

        public virtual void Updating(UpdateContentContext context)
        {
        }

        public virtual void Updated(UpdateContentContext context)
        {
        }

        public virtual void Removing(RemoveContentContext context)
        {
        }

        public virtual void Removed(RemoveContentContext context)
        {
        }

        public virtual void Activating(ActivatingContentContext context)
        {
        }

        public virtual void Activated(ActivatedContentContext context)
        {
        }

        public virtual void BuildDisplay(BuildDisplayContext context)
        {
        }

        public virtual void BuildEditor(BuildEditorContext context)
        {
        }

        public virtual void UpdateEditor(UpdateEditorContext context)
        {
        }


        public virtual void Initializing(InitializingContentContext context)
        {
        }

        public virtual void Initialized(InitializingContentContext context)
        {
        }

        public virtual void Destroying(DestroyContentContext context)
        {
        }

        public virtual void Destroyed(DestroyContentContext context)
        {
        }
    }
}
