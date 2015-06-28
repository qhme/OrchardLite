using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Environment.Extensions.Models
{
    public static class DefaultExtensionTypes
    {
        public const string Module = "Module";

        public static bool IsModule(string extensionType)
        {
            return string.Equals(extensionType, Module, StringComparison.OrdinalIgnoreCase);
        }

    }

}
