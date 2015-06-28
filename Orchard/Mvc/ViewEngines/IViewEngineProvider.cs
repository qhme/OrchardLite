using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Orchard.Mvc.ViewEngines
{
    public class CreateModulesViewEngineParams
    {
        public IEnumerable<string> VirtualPaths { get; set; }
    }


    public interface IViewEngineProvider : ISingletonDependency
    {
        IViewEngine CreateModulesViewEngine(CreateModulesViewEngineParams parameters);

        /// <summary>
        /// Produce a view engine configured to resolve only fully qualified {viewName} parameters
        /// </summary>
        IViewEngine CreateBareViewEngine();
    }
}
