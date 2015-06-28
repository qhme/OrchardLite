using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Environment.Configuration;

namespace Orchard.Setup.Services
{
    public interface ISetupService : IDependency
    {
        ShellSettings Prime();
        void Setup(SetupContext context);

    }
}
