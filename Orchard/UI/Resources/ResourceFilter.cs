using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Orchard.Environment;
using Orchard.Mvc.Filters;

namespace Orchard.UI.Resources
{
    public class ResourceFilter : FilterProvider, IResultFilter
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        //private readonly dynamic _shapeFactory;

        public ResourceFilter(
            IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // should only run on a full view rendering result
            if (!(filterContext.Result is ViewResult))
                return;

            //不能定义位置
            //var writer = filterContext.ParentActionViewContext.Writer;


            //var ctx = _workContextAccessor.GetContext();
            //var head = ctx.Layout.Head;
            //var tail = ctx.Layout.Tail;
            //head.Add(_shapeFactory.Metas());
            //head.Add(_shapeFactory.HeadLinks());
            //head.Add(_shapeFactory.StylesheetLinks());
            //head.Add(_shapeFactory.HeadScripts());
            //tail.Add(_shapeFactory.FootScripts());
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }
    }

}
