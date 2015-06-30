using Orchard.ContentManagement.Records;

namespace Orchard.Settings
{
    public interface ISite : ISettings
    {
        string SiteName { get; }
        string SiteSalt { get; }
        string SuperUser { get; set; }
        string HomePage { get; set; }
        string SiteCulture { get; set; }
        ResourceDebugMode ResourceDebugMode { get; set; }
        int PageSize { get; set; }
        int MaxPageSize { get; set; }
        int MaxPagedCount { get; set; }
    }
}
