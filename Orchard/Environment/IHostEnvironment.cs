using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Environment
{
    /// <summary>
    /// 运行环境
    /// </summary>
    public interface IHostEnvironment
    {
        bool IsFullTrust { get; }
        string MapPath(string virtualPath);

        bool IsAssemblyLoaded(string name);

        void RestartAppDomain();

    }
}
