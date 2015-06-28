using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Users.Models;

namespace Orchard.Users.Handlers
{
    //public class RegistrationSettingsPartHandler : ContentHandler
    //{
    //    public RegistrationSettingsPartHandler()
    //    {
    //        Filters.Add(new ActivatingFilter<RegistrationSettingsPart>("Site"));
    //        Filters.Add(new TemplateFilterForPart<RegistrationSettingsPart>("RegistrationSettings", "Parts/Users.RegistrationSettings", "users"));
    //    }


    //    protected override void GetItemMetadata(GetContentItemMetadataContext context)
    //    {
    //        if (context.ContentItem.ContentType != "Site")
    //            return;
    //        base.GetItemMetadata(context);
    //        context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("Users")));
    //    }
    //}
}