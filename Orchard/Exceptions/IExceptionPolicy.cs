using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Exceptions
{
    public interface IExceptionPolicy
    {
        /// <summary>
        /// return false if the exception should be rethrown by the caller
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        bool HandleException(object sender, Exception exception);
    }
}
