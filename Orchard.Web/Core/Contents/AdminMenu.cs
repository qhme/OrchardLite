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
                    .Add("Content Items", "1", item => item.Action("List", "Admin", new { area = "Contents", id = "" })));

            builder.AddImageSet("contenttypes");
            builder.Add("Content Definition", "1.4.1", menu =>
            {
                menu.LinkToFirstChild(true);
                menu.Add("Content Types", "1", item => item.Action("Index", "ContentTypes", new { area = "Contents" }).LocalNav());
                menu.Add("Content Parts", "2", item => item.Action("ListParts", "ContentTypes", new { area = "Contents" }).LocalNav());
            });
        }
    }
}