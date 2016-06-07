using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement.Records;

namespace Orchard.ContentManagement
{
    public static class ContentExtensions
    {
        public static bool Is<T>(this ContentItem item)
        {
            return item == null ? false : typeof(T) == item.GetType();
        }

        public static T As<T>(this IContent content) where T : IContent
        {
            return content == null ? default(T) : (T)content.ContentItem.Get(typeof(T));
        }

        public static bool Has<T>(this IContent content)
        {
            return content == null ? false : content.ContentItem.Has(typeof(T));
        }
        public static T Get<T>(this IContent content) where T : IContent
        {
            return content == null ? default(T) : (T)content.ContentItem.Get(typeof(T));
        }


        public static IEnumerable<T> AsPart<T>(this IEnumerable<ContentItem> items) where T : IContent
        {
            return items == null ? null : items.Where(item => item.Is<T>()).Select(item => item.As<T>());
        }


    }
}
