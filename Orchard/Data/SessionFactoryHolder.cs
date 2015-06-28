using NHibernate;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using Orchard.Data.Providers;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Environment.ShellBuilders.Models;
using Orchard.Logging;

namespace Orchard.Data
{
    /// <summary>
    /// 为NHibernate创建会话的工厂，整个项目为一个实例
    /// </summary>
    public interface ISessionFactoryHolder : ISingletonDependency
    {
        ISessionFactory GetSessionFactory();
        Configuration GetConfiguration();
        SessionFactoryParameters GetSessionFactoryParameters();
    }

    public class SessionFactoryHolder : ISessionFactoryHolder, IDisposable
    {
        private ISessionFactory _sessionFactory;
        private readonly ShellBlueprint _shellBlueprint;
        private Configuration _configuration;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ISessionConfigurationCache _sessionConfigurationCache;
        private readonly IDataServicesProviderFactory _dataServicesProviderFactory;
        private readonly IDatabaseCacheConfiguration _cacheConfiguration;
        private readonly Func<IEnumerable<ISessionConfigurationEvents>> _configurers;
        private readonly ShellSettings _shellSettings;

        public SessionFactoryHolder(IHostEnvironment hostEnvironment, ISessionConfigurationCache sessionConfigurationCache,
            IDatabaseCacheConfiguration cacheConfiguration,
            ShellBlueprint shellBluePrint,
            Func<IEnumerable<ISessionConfigurationEvents>> configurers,
            ShellSettings shellSettings,
            IDataServicesProviderFactory dataServicesProviderFactory)
        {
            Logger = NullLogger.Instance;
            _hostEnvironment = hostEnvironment;
            _sessionConfigurationCache = sessionConfigurationCache;
            _cacheConfiguration = cacheConfiguration;
            _configurers = configurers;
            _dataServicesProviderFactory = dataServicesProviderFactory;
            _shellSettings = shellSettings;
            _shellBlueprint = shellBluePrint;
        }

        public ILogger Logger { get; set; }


        public ISessionFactory GetSessionFactory()
        {
            lock (this)
            {
                if (_sessionFactory == null)
                {
                    _sessionFactory = BuildSessionFactory();
                }
            }
            return _sessionFactory;
        }

        public Configuration GetConfiguration()
        {
            lock (this)
            {
                if (_configuration == null)
                {
                    _configuration = BuildConfiguration();
                }
            }
            return _configuration;
        }


        public SessionFactoryParameters GetSessionFactoryParameters()
        {
            //var shellPath = _appDataFolder.Combine("Sites", _shellSettings.Name);
            //_appDataFolder.CreateDirectory(shellPath);

            //var shellFolder = _appDataFolder.MapPath(shellPath);

            return new SessionFactoryParameters
          {
              Configurers = _configurers(),
              Provider = _shellSettings.DataProvider,
              ConnectionString = _shellSettings.DataConnectionString,
              RecordDescriptors = _shellBlueprint.Records,
          };
        }


        private ISessionFactory BuildSessionFactory()
        {
            Logger.Debug("Building session factory");

            if (!_hostEnvironment.IsFullTrust)
                NHibernate.Cfg.Environment.UseReflectionOptimizer = false;

            Configuration config = GetConfiguration();
            var result = config.BuildSessionFactory();
            Logger.Debug("Done building session factory");
            return result;
        }

        private Configuration BuildConfiguration()
        {
            Logger.Debug("Building configuration");
            var parameters = GetSessionFactoryParameters();

            var config = _sessionConfigurationCache.GetConfiguration(() =>
                _dataServicesProviderFactory
                    .CreateProvider(parameters)
                    .BuildConfiguration(parameters)
                .Cache(c => _cacheConfiguration.Configure(c))
            );

            #region NH specific optimization
            // cannot be done in fluent config
            // the IsSelectable = false prevents unused ContentPartRecord proxies from being created 
            // for each ContentItemRecord or ContentItemVersionRecord.
            // done for perf reasons - has no other side-effect

            foreach (var persistentClass in config.ClassMappings)
            {
                if (persistentClass.EntityName.StartsWith("Orchard.ContentManagement.Records."))
                {
                    foreach (var property in persistentClass.PropertyIterator)
                    {
                        if (property.Name.EndsWith("Record") && !property.IsBasicPropertyAccessor)
                        {
                            property.IsSelectable = false;
                        }
                    }
                }
            }
            #endregion

            parameters.Configurers.Invoke(c => c.Finished(config), Logger);

            Logger.Debug("Done Building configuration");
            return config;
        }



        public void Dispose()
        {
            if (_sessionFactory != null)
            {
                _sessionFactory.Dispose();
                _sessionFactory = null;
            }
        }
    }
}
