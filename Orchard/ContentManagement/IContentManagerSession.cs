using Orchard.ContentManagement.Records;

namespace Orchard.ContentManagement
{
    public interface IContentManagerSession : IDependency
    {
        void Store(ContentItem item);

        bool RecallContentRecordId(int id, out ContentItem item);

        void Clear();
    }
}
