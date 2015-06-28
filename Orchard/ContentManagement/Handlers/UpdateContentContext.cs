using Orchard.ContentManagement.Records;

namespace Orchard.ContentManagement.Handlers
{
    public class UpdateContentContext : ContentContextBase
    {
        public UpdateContentContext(ContentItem contentItem)
            : base(contentItem)
        {

        }

    }
}