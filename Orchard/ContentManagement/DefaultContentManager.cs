using Autofac;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.Records;
using Orchard.Data;
using Orchard.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement
{
    public class DefaultContentManager : IContentManager
    {
        private readonly IComponentContext _context;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IRepository<ContentItemRecord> _contentItemRepository;
        private readonly IRepository<ContentTypeRecord> _contentTypeRepository;
        private readonly Lazy<IEnumerable<IContentHandler>> _handlers;
        private readonly Func<IContentManagerSession> _contentManagerSession;
        private readonly Lazy<ISessionLocator> _sessionLocator;
        private readonly Lazy<IContentDisplay> _contentDisplay;

        public DefaultContentManager(IComponentContext context,
            IContentDefinitionManager contentDefinitionManager,
           IRepository<ContentItemRecord> contentItemRepository,
           IRepository<ContentTypeRecord> contentTypeRepository,
           Lazy<IEnumerable<IContentHandler>> handlers,
           Lazy<ISessionLocator> sessionLocator,
           Lazy<IContentDisplay> contentDisplay, Func<IContentManagerSession> contentManagerSession)
        {
            _context = context;
            _handlers = handlers;
            Logger = NullLogger.Instance;
            _contentDefinitionManager = contentDefinitionManager;
            _contentManagerSession = contentManagerSession;
            _sessionLocator = sessionLocator;
            _contentItemRepository = contentItemRepository;
            _contentTypeRepository = contentTypeRepository;
            _contentDisplay = contentDisplay;
        }

        public ILogger Logger { get; set; }

        public IEnumerable<IContentHandler> Handlers
        {
            get { return _handlers.Value; }
        }

        public IEnumerable<ContentTypeDefinition> GetContentTypeDefinitions()
        {
            return _contentDefinitionManager.ListTypeDefinitions();
        }

        public ContentItem New(string contentType)
        {
            var contentTypeDefinition = _contentDefinitionManager.GetTypeDefinition(contentType);
            if (contentTypeDefinition == null)
            {
                contentTypeDefinition = new ContentTypeDefinitionBuilder().Named(contentType).Build();
            }

            // create a new kernel for the model instance
            var context = new ActivatingContentContext
            {
                ContentType = contentTypeDefinition.Name,
                Definition = contentTypeDefinition,
                Builder = new ContentItemBuilder(contentTypeDefinition)
            };

            // invoke handlers to weld aspects onto kernel
            Handlers.Invoke(handler => handler.Activating(context), Logger);

            var context2 = new ActivatedContentContext
            {
                ContentType = contentType,
                ContentItem = context.Builder.Build()
            };

            // back-reference for convenience (e.g. getting metadata when in a view)
            context2.ContentItem.ContentManager = this;

            Handlers.Invoke(handler => handler.Activated(context2), Logger);

            var context3 = new InitializingContentContext
            {
                ContentType = context2.ContentType,
                ContentItem = context2.ContentItem,
            };

            Handlers.Invoke(handler => handler.Initializing(context3), Logger);
            Handlers.Invoke(handler => handler.Initialized(context3), Logger);

            // composite result is returned
            return context3.ContentItem;
        }

        public void Create(ContentItem contentItem)
        {
            _contentItemRepository.Create(contentItem.Record);

            // build a context with the initialized instance to create
            var context = new CreateContentContext(contentItem);

            // invoke handlers to add information to persistent stores
            Handlers.Invoke(handler => handler.Creating(context), Logger);
            Handlers.Invoke(handler => handler.Created(context), Logger);
        }

        public ContentItem Get(int id)
        {
            var session = _contentManagerSession();
            ContentItem contentItem;

            if (session.RecallContentRecordId(id, out contentItem))
            {
                // try to reload a previously loaded published content item
                return contentItem;
            }

            contentItem = New(contentItem.ContentType);
            // store in session prior to loading to avoid some problems with simple circular dependencies
            session.Store(contentItem);

            return contentItem;
        }

        public void Remove(ContentItem contentItem)
        {
            var context = new RemoveContentContext(contentItem);

            Handlers.Invoke(handler => handler.Removing(context), Logger);
            Handlers.Invoke(handler => handler.Removed(context), Logger);
        }

        public void Destroy(ContentItem contentItem)
        {
            var session = _sessionLocator.Value.For(typeof(ContentItemRecord));
            var context = new DestroyContentContext(contentItem);

            // Give storage filters a chance to delete content part records.
            Handlers.Invoke(handler => handler.Destroying(context), Logger);

            // Delete the content item record itself.
            session
                .CreateQuery("delete from Orchard.ContentManagement.Records.ContentItemRecord ci where ci.Id = (:id)")
                .SetParameter("id", contentItem.Id)
                .ExecuteUpdate();

            Handlers.Invoke(handler => handler.Destroyed(context), Logger);
        }

        public void Clear()
        {
            var session = _contentManagerSession();
            session.Clear();
        }

        public string BuildDisplay(IContent content, string displayType = "")
        {
            return _contentDisplay.Value.BuildDisplay(content, displayType);
        }

        public string BuildEditor(IContent content)
        {
            return _contentDisplay.Value.BuildEditor(content);
        }

        public string UpdateEditor(IContent content, IUpdateModel updater)
        {
            var context = new UpdateContentContext(content.ContentItem);

            Handlers.Invoke(handler => handler.Updating(context), Logger);

            var result = _contentDisplay.Value.UpdateEditor(content, updater);

            Handlers.Invoke(handler => handler.Updated(context), Logger);

            return result;
        }

        public IContentQuery<ContentItem> Query()
        {
            var query = _context.Resolve<IContentQuery>(TypedParameter.From<IContentManager>(this));
            return query.ForPart<ContentItem>();    
        }
    }
}
