using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Orchard.Environment;

namespace Orchard.Users
{
    public class AutoMapShellEvent : IOrchardShellEvents
    {
        public void Activated()
        {
            Debug.WriteLine(DateTime.Now);
        }

        public void Terminating()
        {

        }
    }
}