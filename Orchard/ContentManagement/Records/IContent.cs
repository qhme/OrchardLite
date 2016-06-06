using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.Records
{
    public interface IContent
    {
        /// <summary>
        /// The ContentItem's identifier.
        /// </summary>
        int Id { get; }

        ContentItem ContentItem { get; }

    }
}
