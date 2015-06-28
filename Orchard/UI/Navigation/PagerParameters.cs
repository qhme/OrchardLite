using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.UI.Navigation
{
    public class PagerParameters
    {
        /// <summary>
        /// Gets or sets the current page number or null if none specified.
        /// </summary>
        public int? Page { get; set; }

        /// <summary>
        /// Gets or sets the current page size or null if none specified.
        /// </summary>
        public int? PageSize { get; set; }
    }

}
