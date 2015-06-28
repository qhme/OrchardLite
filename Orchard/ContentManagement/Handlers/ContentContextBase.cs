using Orchard.ContentManagement.Records;
using Orchard.Logging;

namespace Orchard.ContentManagement.Handlers
{
    public class ContentContextBase
    {
        protected ContentContextBase(ContentItem contentItem)
        {
            ContentItem = contentItem;
            Id = contentItem.Id;
        }

        public int Id { get; private set; }
        public ContentItem ContentItem { get; private set; }
        public ILogger Logger { get; set; }
    }
}