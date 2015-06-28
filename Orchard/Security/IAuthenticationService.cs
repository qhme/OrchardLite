using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Security
{
    /// <summary>
    /// 用户验证服务
    /// </summary>
    public interface IAuthenticationService : IDependency
    {
        void SignIn(IUser user, bool createPersistentCookie);

        void SignOut();

        void SetAuthenticatedUserForRequest(IUser user);

        IUser GetAuthenticatedUser();
    }
}
