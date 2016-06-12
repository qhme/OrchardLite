using Orchard.ContentManagement.MetaData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.Core.Contents.ViewModels
{
    public class EditTypePartViewModel
    {
        public EditTypePartViewModel()
        {

        }

        public EditTypePartViewModel(int index, ContentTypePartDefinition part)
        {
            PartName = part.PartName;
        }

        public int Index
        {
            get;
            set;
        }

        public string PartName { get; set; }
        public string Prefix { get { return "Parts[" + Index + "]"; } }

        public EditTypeViewModel Type { get; set; }
    }
}