using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement
{
    public class DefaultContentManagerSession : IContentManagerSession
    {
        private readonly IDictionary<int, ContentItem> _publishedItemsByContentRecordId = new Dictionary<int, ContentItem>();

        public void Store(ContentItem item)
        {
            _publishedItemsByContentRecordId[item.Id] = item;
        }

        public bool RecallContentRecordId(int id, out ContentItem item)
        {
            return _publishedItemsByContentRecordId.TryGetValue(id, out item);
        }

        public void Clear()
        {
            _publishedItemsByContentRecordId.Clear();
        }
    }
}
