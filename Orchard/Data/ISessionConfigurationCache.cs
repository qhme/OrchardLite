using NHibernate.Cfg;
using System;

namespace Orchard.Data
{
    public interface ISessionConfigurationCache
    {
        Configuration GetConfiguration(Func<Configuration> builder);
    }
}
