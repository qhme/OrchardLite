using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Security
{
    public interface IMembershipService : IDependency
    {
        IUser CreateUser(CreateUserParams createUserParams);
        IUser GetUser(string username);

        IUser GetUser(int id);

        IUser ValidateUser(string userNameOrEmail, string password);

        void SetPassword(IUser user, string password);
    }
}
