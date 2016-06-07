using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement.Records;

namespace Orchard.ContentManagement.Handlers
{
    public abstract class StorageFilterBase<TPart> : IContentStorageFilter where TPart : class,IContent
    {
        protected virtual void Activated(ActivatedContentContext context, TPart instance) { }
        protected virtual void Initializing(InitializingContentContext context, TPart instance) { }
        protected virtual void Initialized(InitializingContentContext context, TPart instance) { }
        protected virtual void Creating(CreateContentContext context, TPart instance) { }
        protected virtual void Created(CreateContentContext context, TPart instance) { }
        protected virtual void Updating(UpdateContentContext context, TPart instance) { }
        protected virtual void Updated(UpdateContentContext context, TPart instance) { }
        protected virtual void Removing(RemoveContentContext context, TPart instance) { }
        protected virtual void Removed(RemoveContentContext context, TPart instance) { }
        protected virtual void Destroying(DestroyContentContext context, TPart instance) { }
        protected virtual void Destroyed(DestroyContentContext context, TPart instance) { }


        void IContentStorageFilter.Creating(CreateContentContext context)
        {
            if (context.ContentItem.Is<TPart>())
                Creating(context, context.ContentItem as TPart);
        }

        void IContentStorageFilter.Created(CreateContentContext context)
        {
            if (context.ContentItem.Is<TPart>())
                Created(context, context.ContentItem as TPart);
        }

        void IContentStorageFilter.Updating(UpdateContentContext context)
        {
            if (context.ContentItem.Is<TPart>())
                Updating(context, context.ContentItem as TPart);
        }

        void IContentStorageFilter.Updated(UpdateContentContext context)
        {
            if (context.ContentItem.Is<TPart>())
                Updated(context, context.ContentItem as TPart);
        }

        void IContentStorageFilter.Removing(RemoveContentContext context)
        {
            if (context.ContentItem.Is<TPart>())
                Removing(context, context.ContentItem as TPart);
        }

        void IContentStorageFilter.Removed(RemoveContentContext context)
        {
            if (context.ContentItem.Is<TPart>())
                Removed(context, context.ContentItem as TPart);
        }

        void IContentStorageFilter.Activated(ActivatedContentContext context)
        {
            if (context.ContentItem.Is<TPart>())
                Activated(context, context.ContentItem.As<TPart>());
        }

        void IContentStorageFilter.Initializing(InitializingContentContext context)
        {
            if (context.ContentItem.Is<TPart>())
                Initializing(context, context.ContentItem.As<TPart>());
        }

        void IContentStorageFilter.Initialized(InitializingContentContext context)
        {
            if (context.ContentItem.Is<TPart>())
                Initialized(context, context.ContentItem.As<TPart>());
        }

        void IContentStorageFilter.Destroying(DestroyContentContext context)
        {
            if (context.ContentItem.Is<TPart>())
                Destroying(context, context.ContentItem.As<TPart>());
        }

        void IContentStorageFilter.Destroyed(DestroyContentContext context)
        {
            if (context.ContentItem.Is<TPart>())
                Destroyed(context, context.ContentItem.As<TPart>());
        }
    }

}
