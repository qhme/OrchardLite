using Orchard.UI.Resources;
namespace Orchard.jQuery
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();
            manifest.DefineScript("jQuery").SetUrl("jquery-1.11.1.min.js").SetVersion("1.11.1");
            manifest.DefineScript("jQueryMigrate").SetUrl("jquery-migrate-1.2.1.min.js").SetVersion("1.2.1");


            // Additional utilities and plugins.
            manifest.DefineScript("jQueryUtils").SetUrl("jquery.utils.js").SetDependencies("jQuery");
            manifest.DefineScript("jQueryPlugin").SetUrl("jquery.plugin.min.js").SetDependencies("jQuery");

            // jQuery Calendars.
            manifest.DefineScript("jQueryCalendars_All").SetUrl("calendars/jquery.calendars.all.min.js").SetDependencies("jQueryPlugin").SetVersion("2.0.0");
            manifest.DefineScript("jQueryCalendars_Picker_Ext").SetUrl("calendars/jquery.calendars.picker.ext.min.js").SetDependencies("jQueryCalendars_Picker").SetVersion("2.0.0");
            manifest.DefineStyle("jQueryCalendars_Picker").SetUrl("jquery.calendars.picker.css").SetVersion("2.0.0");
            manifest.DefineStyle("jQueryUI_Calendars_Picker").SetUrl("ui.calendars.picker.css").SetDependencies("jQueryUI_Orchard").SetVersion("2.0.0");

            // jQuery Time Entry.
            manifest.DefineScript("jQueryTimeEntry").SetUrl("timeentry/jquery.timeentry.min.js").SetDependencies("jQueryPlugin").SetVersion("2.0.1");
            manifest.DefineStyle("jQueryTimeEntry").SetUrl("jquery.timeentry.css").SetVersion("2.0.1");

            // jQuery Date/Time Editor Enhancements.
            manifest.DefineStyle("jQueryDateTimeEditor").SetUrl("jquery-datetime-editor.css").SetDependencies("DateTimeEditor");

            // jQuery File Upload.
            manifest.DefineScript("jQueryIFrameTransport").SetUrl("jquery.iframe-transport.min.js").SetVersion("1.9.0").SetDependencies("jQuery");
            manifest.DefineScript("jQueryFileUpload").SetUrl("jquery.fileupload.min.js").SetVersion("5.41.0").SetDependencies("jQueryIFrameTransport").SetDependencies("jQueryUI_Widget");

            // jQuery Color Box.
            manifest.DefineScript("jQueryColorBox").SetUrl("colorbox/jquery.colorbox.min.js").SetVersion("1.5.13").SetDependencies("jQuery");
            manifest.DefineStyle("jQueryColorBox").SetUrl("colorbox.css").SetVersion("1.5.13");

            // jQuery Cookie.
            manifest.DefineScript("jQueryCookie").SetUrl("jquery.cookie.min.js").SetVersion("1.4.1").SetDependencies("jQuery");

            manifest.DefineScript("jQueryValidate").SetUrl("jquery.validate.min.js").SetDependencies("jQuery");
            manifest.DefineScript("jQueryValidateUnobtrusive").SetUrl("jquery.validate.unobtrusive.min.js").SetDependencies("jQueryValidate");
        }
    }
}
