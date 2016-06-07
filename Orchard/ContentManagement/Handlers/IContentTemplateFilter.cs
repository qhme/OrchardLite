using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.Handlers
{
    public interface IContentTemplateFilter
    {
        //void GetContentItemMetadata(GetContentItemMetadataContext context);
        void BuildDisplayShape(BuildDisplayContext context);
        void BuildEditorShape(BuildEditorContext context);
        void UpdateEditorShape(UpdateEditorContext context);
    }
}
