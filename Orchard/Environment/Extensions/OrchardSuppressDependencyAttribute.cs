using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Environment.Extensions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class OrchardSuppressDependencyAttribute : Attribute
    {
        public OrchardSuppressDependencyAttribute(string fullName)
        {
            FullName = fullName;
        }

        public string FullName { get; set; }
    }
}
