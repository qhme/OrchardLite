using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Environment.Extensions;
using Orchard.UI.Admin.Notification;
using Orchard.UI.Notify;

namespace Orchard.Core.Dashboard.Services
{
    public class CompilationErrorBanner : INotificationProvider
    {
        private readonly ICriticalErrorProvider _errorProvider;

        public CompilationErrorBanner(ICriticalErrorProvider errorProvider)
        {
            _errorProvider = errorProvider;
        }


        public IEnumerable<NotifyEntry> GetNotifications()
        {
            return _errorProvider.GetErrors()
                .Select(message => new NotifyEntry { Message = message, Type = NotifyType.Error });
        }
    }
}
