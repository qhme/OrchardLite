using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Orchard.Security;
using Orchard.Settings;

namespace Orchard
{
    public abstract class WorkContext
    {
        /// <summary>
        /// Resolves a registered dependency type
        /// </summary>
        /// <typeparam name="T">The type of the dependency</typeparam>
        /// <returns>An instance of the dependency if it could be resolved</returns>
        public abstract T Resolve<T>();

        /// <summary>
        /// Tries to resolve a registered dependency type
        /// </summary>
        /// <typeparam name="T">The type of the dependency</typeparam>
        /// <param name="service">An instance of the dependency if it could be resolved</param>
        /// <returns>True if the dependency could be resolved, false otherwise</returns>
        public abstract bool TryResolve<T>(out T service);

        public abstract T GetState<T>(string name);

        public abstract void SetState<T>(string name, T value);

        /// <summary>
        /// The http context corresponding to the work context
        /// </summary>
        public HttpContextBase HttpContext
        {
            get { return GetState<HttpContextBase>("HttpContext"); }
            set { SetState("HttpContext", value); }
        }

        /// <summary>
        /// The user, if there is any corresponding to the work context
        /// </summary>
        public IUser CurrentUser
        {
            get { return GetState<IUser>("CurrentUser"); }
            set { SetState("CurrentUser", value); }
        }

        public ISite CurrentSite
        {
            get { return GetState<ISite>("CurrentSite"); }
            set { SetState("CurrentSite", value); }
        }

    }

}
