using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Orchard.UI
{
    public class OrchardTagBuilder : TagBuilder
    {
        public OrchardTagBuilder(string tagName) : base(tagName) { }

        public IHtmlString StartElement { get { return new HtmlString(ToString(TagRenderMode.StartTag)); } }
        public IHtmlString EndElement { get { return new HtmlString(ToString(TagRenderMode.EndTag)); } }
    }
}
