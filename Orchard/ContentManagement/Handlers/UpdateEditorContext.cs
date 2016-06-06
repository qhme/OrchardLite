using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.Handlers
{
    public class UpdateEditorContext : BuildEditorContext
    {

        public UpdateEditorContext(IContent content, IUpdateModel updater, string path)
            : base(content)
        {

            Updater = updater;
            Path = path;
        }

        public IUpdateModel Updater { get; private set; }
        public string Path { get; private set; }
    }

}
