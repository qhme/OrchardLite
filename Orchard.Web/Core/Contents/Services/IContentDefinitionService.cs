using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Contents.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Core.Contents.Services
{
    public interface IContentDefinitionService : IDependency
    {
        IEnumerable<EditTypeViewModel> GetTypes();
        EditTypeViewModel GetType(string name);
        ContentTypeDefinition AddType(string name, string displayName);
        void AlterType(EditTypeViewModel typeViewModel, IUpdateModel updater);
        void RemoveType(string name, bool deleteContent);
        void AddPartToType(string partName, string typeName);
        void RemovePartFromType(string partName, string typeName);
        string GenerateContentTypeNameFromDisplayName(string displayName);
        IEnumerable<EditPartViewModel> GetParts();
        EditPartViewModel GetPart(string name);
    }
}
