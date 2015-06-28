using Orchard.ContentManagement.Records;

namespace Orchard.ContentManagement.Handlers
{
    public class CreateContentContext : ContentContextBase
    {
        public CreateContentContext(ContentItem contentItem)
            : base(contentItem)
        {
        }

    }
}
