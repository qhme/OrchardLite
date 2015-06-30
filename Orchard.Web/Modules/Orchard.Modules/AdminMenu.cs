using Orchard.Security;
using Orchard.UI.Navigation;

namespace Orchard.Modules
{
    public class AdminMenu : INavigationProvider
    {

        public string MenuName
        {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.AddImageSet("modules")
                .Add("Modules", "9", menu => menu.Action("Features", "Admin", new { area = "Orchard.Modules" }).Permission(Permissions.ManageFeatures)
                    .Add("Features", "0", item => item.Action("Features", "Admin", new { area = "Orchard.Modules" }).Permission(Permissions.ManageFeatures).LocalNav())
                    .Add("Installed", "1", item => item.Action("Index", "Admin", new { area = "Orchard.Modules" }).Permission(StandardPermissions.SiteOwner).LocalNav())
                    //.Add("Recipes", "2", item => item.Action("Recipes", "Admin", new { area = "Orchard.Modules" }).Permission(StandardPermissions.SiteOwner).LocalNav())
                    );
        }
    }
}
