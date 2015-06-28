using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement.Records;

namespace Orchard.ContentManagement.Handlers
{
    public abstract class StorageFilterBase<TContent> : IContentStorageFilter where TContent : ContentItem
    {
        protected virtual void Creating(CreateContentContext context, TContent instance) { }
        protected virtual void Created(CreateContentContext context, TContent instance) { }
        protected virtual void Updating(UpdateContentContext context, TContent instance) { }
        protected virtual void Updated(UpdateContentContext context, TContent instance) { }
        protected virtual void Removing(RemoveContentContext context, TContent instance) { }
        protected virtual void Removed(RemoveContentContext context, TContent instance) { }

        void IContentStorageFilter.Creating(CreateContentContext context)
        {
            if (context.ContentItem.Is<TContent>())
                Creating(context, context.ContentItem as TContent);
        }

        void IContentStorageFilter.Created(CreateContentContext context)
        {
            if (context.ContentItem.Is<TContent>())
                Created(context, context.ContentItem as TContent);
        }

        void IContentStorageFilter.Updating(UpdateContentContext context)
        {
            if (context.ContentItem.Is<TContent>())
                Updating(context, context.ContentItem as TContent);
        }

        void IContentStorageFilter.Updated(UpdateContentContext context)
        {
            if (context.ContentItem.Is<TContent>())
                Updated(context, context.ContentItem as TContent);
        }

        void IContentStorageFilter.Removing(RemoveContentContext context)
        {
            if (context.ContentItem.Is<TContent>())
                Removing(context, context.ContentItem as TContent);
        }

        void IContentStorageFilter.Removed(RemoveContentContext context)
        {
            if (context.ContentItem.Is<TContent>())
                Removed(context, context.ContentItem as TContent);
        }
    }

}
