using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Environment;
using Orchard.Security.Permissions;
using Orchard.UI.Notify;

namespace Orchard.Security
{
    /// <summary>
    /// Authorization services for the current user
    /// </summary>
    public interface IAuthorizer : IDependency
    {
        /// <summary>
        /// Authorize the current user against a permission
        /// </summary>
        /// <param name="permission">A permission to authorize against</param>
        bool Authorize(Permission permission);

        /// <summary>
        /// Authorize the current user against a permission; if authorization fails, the specified
        /// message will be displayed
        /// </summary>
        /// <param name="permission">A permission to authorize against</param>
        /// <param name="message">A localized message to display if authorization fails</param>
        bool Authorize(Permission permission, string message);
    }

    public class Authorizer : IAuthorizer
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly INotifier _notifier;
        private readonly IWorkContextAccessor _workContextAccessor;

        public Authorizer(
            IAuthorizationService authorizationService,
            INotifier notifier,
            IWorkContextAccessor workContextAccessor)
        {
            _authorizationService = authorizationService;
            _notifier = notifier;
            _workContextAccessor = workContextAccessor;
        }


        public bool Authorize(Permission permission)
        {
            return Authorize(permission, null);
        }

        public bool Authorize(Permission permission, string message)
        {
            if (_authorizationService.TryCheckAccess(permission, _workContextAccessor.GetContext().CurrentUser))
                return true;

            if (message != null)
            {
                if (_workContextAccessor.GetContext().CurrentUser == null)
                {
                    _notifier.Error(string.Format("{0}. Anonymous users do not have {1} permission.",
                                      message, permission.Name));
                }
                else
                {
                    _notifier.Error(string.Format("{0}. Current user, {2}, does not have {1} permission.",
                                      message, permission.Name, _workContextAccessor.GetContext().CurrentUser.UserName));
                }
            }

            return false;
        }
    }


}
