using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Orchard.Logging;

namespace Orchard.Mvc.ViewEngines.Razor
{
    public class RazorViewEngineProvider : IViewEngineProvider
    {
        public RazorViewEngineProvider()
        {
            Logger = NullLogger.Instance;
            RazorCompilationEventsShim.EnsureInitialized();
        }
        static readonly string[] DisabledFormats = new[] { "~/Disabled" };

        public ILogger Logger { get; set; }

        public IViewEngine CreateModulesViewEngine(CreateModulesViewEngineParams parameters)
        {
            var areaFormats = new[] {
                                        "~/Core/{2}/Views/{1}/{0}.cshtml",
                                        "~/Modules/{2}/Views/{1}/{0}.cshtml"
                                     };

            //Logger.Debug("AreaFormats (module): \r\n\t-{0}", string.Join("\r\n\t-", areaFormats));
            //基本上不再用了
            //var universalFormats = parameters.VirtualPaths
            //    .SelectMany(x => new[] {
            //                               x + "/Views/{0}.cshtml",
            //                            })
            //    .ToArray();
            var viewFormat = new[] { 
                "~/Views/{1}/{0}.cshtml",
                "~/Views/{1}/{0}.cshtml"
            };

            var masterLocationFormat = new[] { 
                "~/Views/Shared/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml"
            };

            var areaMasterLocationFormats = new[] {
                   "~/Core/{2}/Views/Shared/{0}.cshtml",
                   "~/Modules/{2}/Views/Shared/{0}.cshtml"
             }.ToList();


            var partialViewFormats = parameters.VirtualPaths
                  .SelectMany(x => new[] {
                                           x + "/Views/Shared/{0}.cshtml",
                                       }).ToArray();
            ;

            areaMasterLocationFormats.AddRange(parameters.VirtualPaths
                  .SelectMany(x => new[] {
                                           x + "/Views/Shared/{0}.cshtml",
                                       }));



            //Logger.Debug("UniversalFormats (module): \r\n\t-{0}", string.Join("\r\n\t-", universalFormats));
            var viewEngine = new RazorViewEngine
           {
               MasterLocationFormats = masterLocationFormat,
               ViewLocationFormats = viewFormat,
               PartialViewLocationFormats = partialViewFormats,
               AreaMasterLocationFormats = areaMasterLocationFormats.ToArray(),
               AreaViewLocationFormats = areaFormats,
               AreaPartialViewLocationFormats = areaFormats,
           };

            return viewEngine;
        }

        public IViewEngine CreateBareViewEngine()
        {
            return new RazorViewEngine
            {
                MasterLocationFormats = DisabledFormats,
                ViewLocationFormats = DisabledFormats,
                PartialViewLocationFormats = DisabledFormats,
                AreaMasterLocationFormats = DisabledFormats,
                AreaViewLocationFormats = DisabledFormats,
                AreaPartialViewLocationFormats = DisabledFormats,
            };
        }
    }

}
