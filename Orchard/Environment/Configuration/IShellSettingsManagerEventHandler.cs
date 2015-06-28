using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Events;

namespace Orchard.Environment.Configuration
{
    public interface IShellSettingsManagerEventHandler : IEventHandler
    {
        void Saved(ShellSettings settings);
    }
}
