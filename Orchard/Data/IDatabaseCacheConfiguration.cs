using NHibernate.Cfg.Loquacious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data
{
    /// <summary>
    ///缓存NHibernate的数据库配置，每个会话实例化
    /// </summary>
    public interface IDatabaseCacheConfiguration : IDependency
    {
        void Configure(ICacheConfigurationProperties cache);
    }
}
