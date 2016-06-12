using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.MetaData
{
    public interface IContentTypeDefinition : IDependency
    {
        string TypeName { get; }
        string Description { get; }
    }

    public abstract class ContentTypeDefintion<T> : IContentTypeDefinition where T : ContentPartRecord
    {
        public string TypeName { get { return typeof(T).Name; } }

        public virtual string Description { get { return string.Empty; } }
    }
}
