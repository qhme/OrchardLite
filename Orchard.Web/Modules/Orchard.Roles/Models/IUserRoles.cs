using System.Collections.Generic;

namespace Orchard.Roles.Models
{
    public interface IUserRoles
    {
        IList<string> Roles { get; }
    }
}