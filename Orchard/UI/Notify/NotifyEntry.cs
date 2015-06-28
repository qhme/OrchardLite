using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.UI.Notify
{
    public enum NotifyType
    {
        Information,
        Warning,
        Error
    }

    public class NotifyEntry
    {
        public NotifyType Type { get; set; }
        public string Message { get; set; }
    }
}
