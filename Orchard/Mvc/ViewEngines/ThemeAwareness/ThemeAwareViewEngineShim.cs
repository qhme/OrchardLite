using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Orchard.Environment;

namespace Orchard.Mvc.ViewEngines.ThemeAwareness
{
    public class ThemeAwareViewEngineShim : IViewEngine, IShim
    {
        public ThemeAwareViewEngineShim()
        {
            OrchardHostContainerRegistry.RegisterShim(this);
        }

        public IOrchardHostContainer HostContainer { get; set; }

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            return Forward(
                 controllerContext,
                 dve => dve.FindPartialView(controllerContext, partialViewName, useCache),
                 EmptyViewEngineResult);

        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            return Forward(
                 controllerContext,
                 dve => dve.FindView(controllerContext, viewName, masterName, useCache),
                 EmptyViewEngineResult);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            throw new NotImplementedException();
        }

        static TResult Forward<TResult>(ControllerContext controllerContext, Func<IThemeAwareViewEngine, TResult> forwardAction, Func<TResult> defaultAction)
        {
            var workContext = controllerContext.GetWorkContext();
            if (workContext != null)
            {
                var displayViewEngine = workContext.Resolve<IThemeAwareViewEngine>();
                if (displayViewEngine != null)
                {
                    return forwardAction(displayViewEngine);
                }
            }
            return defaultAction();
        }

        static ViewEngineResult EmptyViewEngineResult()
        {
            return new ViewEngineResult(Enumerable.Empty<string>());
        }

    }
}
