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
        public EditPartViewModel()
        {
            Settings = new SettingsDictionary();
        }

        public EditPartViewModel(ContentPartDefinition contentPartDefinition)
        {
            Name = contentPartDefinition.Name;
            Settings = contentPartDefinition.Settings;
            _Definition = contentPartDefinition;
        }

        public string Prefix { get { return "PartDefinition"; } }
        public string Name { get; set; }

        private string _displayName;
        [Required]
        public string DisplayName
        {
            get { return !string.IsNullOrWhiteSpace(_displayName) ? _displayName : Name.TrimEnd("Part").CamelFriendly(); }
            set { _displayName = value; }
        }

        public string Description
        {
            get { return Settings.ContainsKey("ContentPartSettings.Description") ? Settings["ContentPartSettings.Description"] : null; }
            set { Settings["ContentPartSettings.Description"] = value; }
        }
  
        public SettingsDictionary Settings { get; set; }
        public ContentPartDefinition _Definition { get; private set; }
    }
}