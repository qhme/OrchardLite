using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Security
{
    [Serializable]
    public class OrchardSecurityException : CoreException
    {
        public OrchardSecurityException(string message) : base(message) { }
        public OrchardSecurityException(string message, Exception innerException) : base(message, innerException) { }
        protected OrchardSecurityException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public string PermissionName { get; set; }
        public IUser User { get; set; }
    }

}
