using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement.Records;
using Orchard.Logging;

namespace Orchard.ContentManagement.Handlers
{
    /// <summary>
    /// 由于移除了ContentManager,handlers暂时不会起作用
    /// </summary>
    public abstract class ContentHandler : IContentHandler
    {
        protected ContentHandler()
        {
            Filters = new List<IContentFilter>();
            Logger = NullLogger.Instance;
        }

        public List<IContentFilter> Filters { get; set; }
        public ILogger Logger { get; set; }

        protected void OnActivated<TPart>(Action<ActivatedContentContext, TPart> handler) where TPart : class, IContent
        {
            Filters.Add(new InlineStorageFilter<TPart> { OnActivated = handler });
        }

        protected void OnCreating<TContent>(Action<CreateContentContext, TContent> handler) where TContent : class, IContent
        {
            Filters.Add(new InlineStorageFilter<TContent> { OnCreating = handler });
        }

        protected void OnCreated<TContent>(Action<CreateContentContext, TContent> handler) where TContent : class,IContent
        {
            Filters.Add(new InlineStorageFilter<TContent> { OnCreated = handler });
        }

        protected void OnUpdating<TContent>(Action<UpdateContentContext, TContent> handler) where TContent : class, IContent
        {
            Filters.Add(new InlineStorageFilter<TContent> { OnUpdating = handler });
        }

        protected void OnUpdated<TContent>(Action<UpdateContentContext, TContent> handler) where TContent : class, IContent
        {
            Filters.Add(new InlineStorageFilter<TContent> { OnUpdated = handler });
        }

        protected void OnRemoving<TContent>(Action<RemoveContentContext, TContent> handler) where TContent : class, IContent
        {
            Filters.Add(new InlineStorageFilter<TContent> { OnRemoving = handler });
        }

        protected void OnRemoved<TContent>(Action<RemoveContentContext, TContent> handler) where TContent : class, IContent
        {
            Filters.Add(new InlineStorageFilter<TContent> { OnRemoved = handler });
        }



        class InlineStorageFilter<TPart> : StorageFilterBase<TPart> where TPart : class,IContent
        {
            public Action<ActivatedContentContext, TPart> OnActivated { get; set; }

            public Action<CreateContentContext, TPart> OnCreating { get; set; }
            public Action<CreateContentContext, TPart> OnCreated { get; set; }
            public Action<UpdateContentContext, TPart> OnUpdating { get; set; }
            public Action<UpdateContentContext, TPart> OnUpdated { get; set; }

            public Action<RemoveContentContext, TPart> OnRemoving { get; set; }
            public Action<RemoveContentContext, TPart> OnRemoved { get; set; }

            protected override void Creating(CreateContentContext context, TPart instance)
            {
                if (OnCreating != null) OnCreating(context, instance);
            }
            protected override void Created(CreateContentContext context, TPart instance)
            {
                if (OnCreated != null) OnCreated(context, instance);
            }

            protected override void Updating(UpdateContentContext context, TPart instance)
            {
                if (OnUpdating != null) OnUpdating(context, instance);
            }
            protected override void Updated(UpdateContentContext context, TPart instance)
            {
                if (OnUpdated != null) OnUpdated(context, instance);
            }

            protected override void Removing(RemoveContentContext context, TPart instance)
            {
                if (OnRemoving != null) OnRemoving(context, instance);
            }
            protected override void Removed(RemoveContentContext context, TPart instance)
            {
                if (OnRemoved != null) OnRemoved(context, instance);
            }
        }


        void IContentHandler.Creating(CreateContentContext context)
        {
            foreach (var filter in Filters.OfType<IContentStorageFilter>())
                filter.Creating(context);
            Creating(context);
        }

        void IContentHandler.Created(CreateContentContext context)
        {
            foreach (var filter in Filters.OfType<IContentStorageFilter>())
                filter.Created(context);
            Created(context);
        }


        void IContentHandler.Updating(UpdateContentContext context)
        {
            foreach (var filter in Filters.OfType<IContentStorageFilter>())
                filter.Updating(context);
            Updating(context);
        }

        void IContentHandler.Updated(UpdateContentContext context)
        {
            foreach (var filter in Filters.OfType<IContentStorageFilter>())
                filter.Updated(context);
            Updated(context);
        }


        void IContentHandler.Removing(RemoveContentContext context)
        {
            foreach (var filter in Filters.OfType<IContentStorageFilter>())
                filter.Removing(context);
            Removing(context);
        }

        void IContentHandler.Removed(RemoveContentContext context)
        {
            foreach (var filter in Filters.OfType<IContentStorageFilter>())
                filter.Removed(context);
            Removed(context);
        }

        void IContentHandler.Activating(ActivatingContentContext context)
        {
            foreach (var filter in Filters.OfType<IContentActivatingFilter>())
                filter.Activating(context);
            Activating(context);
        }

        void IContentHandler.Activated(ActivatedContentContext context)
        {
            foreach (var filter in Filters.OfType<IContentStorageFilter>())
                filter.Activated(context);
            Activated(context);
        }

        void IContentHandler.BuildDisplay(BuildDisplayContext context)
        {
            foreach (var filter in Filters.OfType<IContentTemplateFilter>())
                filter.BuildDisplayShape(context);
            BuildDisplayShape(context);
        }

        void IContentHandler.BuildEditor(BuildEditorContext context)
        {
            foreach (var filter in Filters.OfType<IContentTemplateFilter>())
                filter.BuildEditorShape(context);
            BuildEditorShape(context);
        }

        void IContentHandler.UpdateEditor(UpdateEditorContext context)
        {
            foreach (var filter in Filters.OfType<IContentTemplateFilter>())
                filter.UpdateEditorShape(context);
            UpdateEditorShape(context);
        }

        void IContentHandler.Initializing(InitializingContentContext context)
        {
            foreach (var filter in Filters.OfType<IContentStorageFilter>())
                filter.Initializing(context);
            Initializing(context);
        }

        void IContentHandler.Initialized(InitializingContentContext context)
        {
            foreach (var filter in Filters.OfType<IContentStorageFilter>())
                filter.Initialized(context);
            Initialized(context);
        }

        void IContentHandler.Destroying(DestroyContentContext context)
        {
            foreach (var filter in Filters.OfType<IContentStorageFilter>())
                filter.Destroying(context);
            Destroying(context);
        }

        void IContentHandler.Destroyed(DestroyContentContext context)
        {
            foreach (var filter in Filters.OfType<IContentStorageFilter>())
                filter.Destroyed(context);
            Destroyed(context);
        }


        protected virtual void Activating(ActivatingContentContext context) { }

        protected virtual void Activated(ActivatedContentContext context) { }
         protected virtual void Initializing(InitializingContentContext context) { }
        protected virtual void Initialized(InitializingContentContext context) { }

        protected virtual void Creating(CreateContentContext context) { }
        protected virtual void Created(CreateContentContext context) { }

        protected virtual void Updating(UpdateContentContext context) { }
        protected virtual void Updated(UpdateContentContext context) { }

        protected virtual void Removing(RemoveContentContext context) { }
        protected virtual void Removed(RemoveContentContext context) { }
        protected virtual void Destroying(DestroyContentContext context) { }
        protected virtual void Destroyed(DestroyContentContext context) { }

        protected virtual void BuildDisplayShape(BuildDisplayContext context) { }

        protected virtual void BuildEditorShape(BuildEditorContext context) { }

        protected virtual void UpdateEditorShape(UpdateEditorContext context) { }
    }

}
