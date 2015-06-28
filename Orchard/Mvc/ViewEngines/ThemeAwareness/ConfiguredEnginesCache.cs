using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Orchard.Mvc.ViewEngines.ThemeAwareness
{
    public interface IConfiguredEnginesCache : ISingletonDependency
    {
        IViewEngine BindBareEngines(Func<IViewEngine> factory);

        IViewEngine BindDeepEngines(Func<IViewEngine> factory);

    }


    public class ConfiguredEnginesCache : IConfiguredEnginesCache
    {
        IViewEngine _bare;
        IViewEngine _deep;


        public ConfiguredEnginesCache()
        {
        }


        public IViewEngine BindBareEngines(Func<IViewEngine> factory)
        {
            return _bare ?? (_bare = factory());
        }


        public IViewEngine BindDeepEngines(Func<IViewEngine> factory)
        {
            return _deep ?? (_deep = factory());
        }

    }
}
