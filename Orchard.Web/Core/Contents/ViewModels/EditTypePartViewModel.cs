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
            Settings = new SettingsDictionary();
        }

        public EditTypePartViewModel(int index, ContentTypePartDefinition part)
        {
            Index = index;
            PartDefinition = new EditPartViewModel(part.PartName);
            Settings = part.Settings;
            _Definition = part;
        }

        public int Index { get; set; }
        public string Prefix { get { return "Parts[" + Index + "]"; } }
        public EditPartViewModel PartDefinition { get; set; }
        public SettingsDictionary Settings { get; set; }
        public EditTypeViewModel Type { get; set; }
        public ContentTypePartDefinition _Definition { get; private set; }


    }
}