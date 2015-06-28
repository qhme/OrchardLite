using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.UI.Notify
{
    public static class NotifierExtensions
    {
        /// <summary>
        /// Adds a new UI notification of type Information
        /// </summary>
        /// <param name="message">message</param>
        public static void Information(this INotifier notifier, string message)
        {
            notifier.Add(NotifyType.Information, message);
        }

        /// <summary>
        /// Adds a new UI notification of type Warning
        /// </summary>
        /// <param name="message">message</param>
        public static void Warning(this INotifier notifier, string message)
        {
            notifier.Add(NotifyType.Warning, message);
        }

        /// <summary>
        /// Adds a new UI notification of type Error
        /// </summary>
        /// <param name="message">message</param>
        public static void Error(this INotifier notifier, string message)
        {
            notifier.Add(NotifyType.Error, message);
        }
    }
}
