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
    }
}
