using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Security
{
    /// <summary>
    /// 提供当前用户的接口
    /// </summary>
    public interface IUser
    {
        int Id { get; set; }
        string UserName { get; set; }

        string Email { get; set; }
    }
}
