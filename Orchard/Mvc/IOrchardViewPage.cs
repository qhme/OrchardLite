using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Orchard.UI;
using Orchard.UI.Resources;

namespace Orchard.Mvc
{
    /// <summary>
    /// This interface ensures all base view pages implement the 
    /// same set of additional members
    /// </summary>
    public interface IOrchardViewPage
    {
        ScriptRegister Script { get; }
        ResourceRegister Style { get; }

        WorkContext WorkContext { get; }

        IDisposable Capture(Action<IHtmlString> callback);

        void RegisterLink(LinkEntry link);
        void SetMeta(string name, string content, string httpEquiv, string charset);
        void SetMeta(MetaEntry meta);
        void AppendMeta(string name, string content, string contentSeparator);
        void AppendMeta(MetaEntry meta, string contentSeparator);

        bool HasText(object thing);

        IDisplayHelper DisplayHelper { get; }
    }
}
