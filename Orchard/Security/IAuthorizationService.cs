using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Security.Permissions;

namespace Orchard.Security
{
    /// <summary>
    /// Role based system
    /// </summary>
    public interface IAuthorizationService : IDependency
    {
        void CheckAccess(Permission permission, IUser user);
        bool TryCheckAccess(Permission permission, IUser user);
    }
}
