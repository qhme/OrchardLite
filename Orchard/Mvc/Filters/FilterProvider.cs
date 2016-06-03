using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Orchard.Mvc.Filters
{
    public interface IFilterProvider : IDependency
    {
    }

    public abstract class FilterProvider : IFilterProvider
    {

    }
}
