using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Environment.Extensions.Models;

namespace Orchard.Security.Permissions
{
    public interface IPermissionProvider : IDependency
    {
        Feature Feature { get; }

        IEnumerable<Permission> GetPermissions();
        IEnumerable<PermissionStereotype> GetDefaultStereotypes();
    }

    public class PermissionStereotype
    {
        public string Name { get; set; }
        public IEnumerable<Permission> Permissions { get; set; }
    }
}
