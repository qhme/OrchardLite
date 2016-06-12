using Orchard.ContentManagement.MetaData.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Orchard.Utility.Extensions;
using Orchard.Core.Contents.Extensions;

namespace Orchard.Core.Contents.ViewModels
{
    public class EditPartViewModel
    {
        public EditPartViewModel(string partName)
        {
            Name = partName;
        }

        public string Prefix { get { return "PartDefinition"; } }
        public string Name { get; set; }

    }
}