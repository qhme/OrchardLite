using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace Orchard.Core.Dashboard
{
    public class AdminMenu : INavigationProvider
    {
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.AddImageSet("dashboard")
                .Add("Dashboard", "-5",
                    menu => menu.Add("Orchard", "-5",
                        item => item
                            .Action("Index", "Admin", new { area = "Dashboard" })
                            .Permission(StandardPermissions.AccessAdminPanel)));
        }
    }

}
