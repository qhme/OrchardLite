using System.Collections.Generic;
using Orchard.Security;

namespace Orchard.Roles.Models
{
    public static class UserSimulation
    {
        public static IUser Create(string role)
        {
            return new SimulatedUser();
        }

        class SimulatedUser : IUser
        {
            public int Id { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
        }

        class SimulatedUserRoles : IUserRoles
        {
            public IList<string> Roles { get; set; }
        }
    }
}
