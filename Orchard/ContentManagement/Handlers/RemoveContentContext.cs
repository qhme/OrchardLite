using Orchard.ContentManagement.Records;
namespace Orchard.ContentManagement.Handlers
{
    public class RemoveContentContext : ContentContextBase
    {
        public RemoveContentContext(ContentItem contentItem)
            : base(contentItem)
        {
        }
    }
}