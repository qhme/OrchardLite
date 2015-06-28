using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.UI.Navigation
{
    public interface INavigationProvider : IDependency
    {
        string MenuName { get; }
        void GetNavigation(NavigationBuilder builder);
    }
}
