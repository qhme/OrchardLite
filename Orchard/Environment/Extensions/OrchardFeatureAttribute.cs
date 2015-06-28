using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Environment.Extensions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class OrchardFeatureAttribute : Attribute
    {
        public OrchardFeatureAttribute(string text)
        {
            FeatureName = text;
        }

        public string FeatureName { get; set; }
    }
}
