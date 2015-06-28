using NHibernate;
using System;

namespace Orchard.Data
{
    /// <summary>
    /// NHibernate的会话连接器,每个会话实例化一次
    /// </summary>
    public interface ISessionLocator : IDependency
    {
        ISession For(Type entityType);
    }
}
