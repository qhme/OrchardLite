using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.Drivers
{
    public interface IContentPartDriver : IDependency
    {
        DriverResult BuildDisplay(BuildDisplayContext context);
        DriverResult BuildEditor(BuildEditorContext context);
        DriverResult UpdateEditor(UpdateEditorContext context);
        IEnumerable<ContentPartInfo> GetPartInfo();
    }
}
