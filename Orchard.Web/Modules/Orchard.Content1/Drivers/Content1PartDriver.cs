using Orchard.Content1.Models2;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.Content1.Drivers
{
    public class Content1PartDriver : ContentPartDriver<C1Part>
    {
        protected override DriverResult Display(C1Part part, string displayType)
        {
            //return ContentShape("Parts_Blogs_BlogArchives",
            //                   () =>
            //                   {
            //                       var blog = _blogService.Get(part.BlogId, VersionOptions.Published).As<BlogPart>();

            //                       if (blog == null)
            //                           return null;

            //                       return shapeHelper.Parts_Blogs_BlogArchives(Blog: blog, Archives: _blogPostService.GetArchives(blog));
            //                   });
            return base.Display(part, displayType);
        }

        protected override DriverResult Editor(C1Part part)
        {
            //var viewModel = new BlogArchivesViewModel
            //{
            //    BlogId = part.BlogId,
            //    Blogs = _blogService.Get().ToList().OrderBy(b => _contentManager.GetItemMetadata(b).DisplayText)
            //};

            //return ContentShape("Parts_Blogs_BlogArchives_Edit",
            //                    () => shapeHelper.EditorTemplate(TemplateName: "Parts.Blogs.BlogArchives", Model: viewModel, Prefix: Prefix));
            
            return base.Editor(part);
        }

        protected override DriverResult Editor(C1Part part, IUpdateModel updater)
        {
            //var viewModel = new BlogArchivesViewModel();
            //if (updater.TryUpdateModel(viewModel, Prefix, null, null))
            //{
            //    part.BlogId = viewModel.BlogId;
            //}
            return base.Editor(part, updater);
        }
    }
}