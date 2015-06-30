using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement.Records;
using Orchard.Settings;

namespace Orchard.Core.Settings.Models
{
    public class SiteSettings : ISite
    {

        public string SiteName
        {
            get;
            set;
        }

        public string SiteSalt
        {
            get;
            set;
        }

        public string SuperUser
        {
            get;
            set;
        }

        public string HomePage
        {
            get;
            set;
        }


        public ResourceDebugMode ResourceDebugMode
        {
            get;
            set;
        }



        public int PageSize
        {
            get;
            set;
        }

        public int MaxPageSize
        {
            get;
            set;
        }

        public int MaxPagedCount
        {
            get;
            set;
        }


        public string SiteCulture
        {
            get;
            set;
        }
    }
}
