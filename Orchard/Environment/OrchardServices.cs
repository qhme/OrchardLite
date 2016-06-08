using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Data;
using Orchard.Security;
using Orchard.UI.Notify;
using Orchard.ContentManagement;

namespace Orchard.Environment
{
    public class OrchardServices : IOrchardServices
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        public OrchardServices(IContentManager contentManager,
            ITransactionManager transactionManager,
            IAuthorizer authorizer,
            INotifier notifier,
             IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
            ContentManager = contentManager;
            TransactionManager = transactionManager;
            Authorizer = authorizer;
            Notifier = notifier;
        }

        public ITransactionManager TransactionManager { get; private set; }
        public IAuthorizer Authorizer { get; private set; }
        public INotifier Notifier { get; private set; }

        public WorkContext WorkContext { get { return _workContextAccessor.GetContext(); } }

        public IContentManager ContentManager
        {
            get;
            private set;
        }
    }
}
