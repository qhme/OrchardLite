using Orchard.UI.Navigation;
using Orchard.Security;

namespace Orchard.Roles
{
    public class AdminMenu : INavigationProvider
    {
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add("Users",
                menu => menu.Add("Roles", "2.0", item => item.Action("Index", "Admin", new { area = "Orchard.Roles" })
                    .LocalNav().Permission(Permissions.ManageRoles)));
        }
    }
}
