using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.Core.Contents
{
    public class AdminMenu : INavigationProvider
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentManager _contentManager;

        public AdminMenu(IContentDefinitionManager contentDefinitionManager, IContentManager contentManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
            _contentManager = contentManager;
        }


        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            var contentTypeDefinitions = _contentDefinitionManager.ListTypeDefinitions().OrderBy(d => d.Name);
            builder.AddImageSet("content")
                .Add("Content", "1.4", menu => menu
                    .Add("Content Items", "1", item => item.Action("List", "Admin", new { area = "Contents", id = "" }).LocalNav()));
        }
    }
}