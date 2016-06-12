using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Utility.Extensions;

namespace Orchard.ContentManagement.MetaData
{
    public interface IContentDefinitionManager : IDependency
    {
        IEnumerable<ContentTypeDefinition> ListTypeDefinitions();

        ContentTypeDefinition GetTypeDefinition(string name);

        void StoreTypeDefinition(ContentTypeDefinition contentTypeDefinition);
    }

    public static class ContentDefinitionManagerExtensions
    {
        public static void AlterTypeDefinition(this IContentDefinitionManager manager, string name, Action<ContentTypeDefinitionBuilder> alteration)
        {
            var typeDefinition = manager.GetTypeDefinition(name) ?? new ContentTypeDefinition(name);
            var builder = new ContentTypeDefinitionBuilder(typeDefinition);
            alteration(builder);
            manager.StoreTypeDefinition(builder.Build());
        }
    }

}
