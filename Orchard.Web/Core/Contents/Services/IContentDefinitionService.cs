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

        void AlterType(EditTypeViewModel typeViewModel, IUpdateModel updater);

        IEnumerable<EditPartViewModel> GetParts();
    }
}
