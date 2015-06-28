using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.Providers
{
    public interface ISqlStatementProvider : ISingletonDependency
    {
        string DataProvider { get; }
        string GetStatement(string command);
    }

}
