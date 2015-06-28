using Orchard;
using Orchard.Core.Mvc;

namespace Orchard.Core
{
    public class ResourceDisplayName : System.ComponentModel.DisplayNameAttribute, IModelAttribute
    {
        private string _resourceValue = string.Empty;
        //private bool _resourceValueRetrived;

        public ResourceDisplayName(string resourceKey)
            : base(resourceKey)
        {
            ResourceKey = resourceKey;
        }

        public string ResourceKey { get; set; }


        public string Name
        {
            get { return "ResourceDisplayName"; }
        }
    }
}
