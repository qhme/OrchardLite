using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data
{
    /// <summary>
    ///一个NHibernate Session 拦截器，每个会话实例化。
    /// </summary>
    public interface ISessionInterceptor : IInterceptor, IDependency
    {

    }
}
