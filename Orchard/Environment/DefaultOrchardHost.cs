using System.Collections.Generic;
using Orchard.Caching;
using Orchard.Environment.Configuration;
using Orchard.Environment.Descriptor;
using Orchard.Environment.Descriptor.Models;
using Orchard.Environment.Extensions;
using Orchard.Environment.ShellBuilders;
using Orchard.Environment.State;
using Orchard.Logging;
using System.Linq;

namespace Orchard.Environment
{
    public class DefaultOrchardHost : IOrchardHost, IShellSettingsManagerEventHandler, IShellDescriptorManagerEventHandler
    {
        private readonly IHostLocalRestart _hostLocalRestart;
        private readonly IShellContextFactory _shellContextFactory;
        private readonly IProcessingEngine _processingEngine;
        private readonly IShellSettingsManager _shellSettingsManager;
        private readonly IExtensionLoaderCoordinator _extensionLoaderCoordinator;
        private readonly IExtensionMonitoringCoordinator _extensionMonitoringCoordinator;
        private readonly ICacheManager _cacheManager;
        private readonly static object _syncLock = new object();
        private readonly static object _shellContextsWriteLock = new object();

        private IEnumerable<ShellContext> _shellContexts;

        private readonly ContextState<IList<ShellSettings>> _tenantsToRestart;

        public DefaultOrchardHost(
            IShellContextFactory shellContextFactory,
            IProcessingEngine processingEngine,
            IExtensionLoaderCoordinator extensionLoaderCoordinator,
           IExtensionMonitoringCoordinator extensionMonitoringCoordinator,
           ICacheManager cacheManager,
            IShellSettingsManager shellSettingsManager,
           IHostLocalRestart hostLocalRestart)
        {
            _shellContextFactory = shellContextFactory;
            _extensionLoaderCoordinator = extensionLoaderCoordinator;
            _extensionMonitoringCoordinator = extensionMonitoringCoordinator;
            _cacheManager = cacheManager;
            _hostLocalRestart = hostLocalRestart;
            _processingEngine = processingEngine;
            _shellSettingsManager = shellSettingsManager;
            Logger = NullLogger.Instance;

            _tenantsToRestart = new ContextState<IList<ShellSettings>>("DefaultOrchardHost.TenantsToRestart", () => new List<ShellSettings>());
        }

        public ILogger Logger { get; set; }

        public ShellContext GetShellContext(ShellSettings settings)
        {
            return BuildCurrent().FirstOrDefault();
        }

        void IOrchardHost.Initialize()
        {
            Logger.Information("Initializing");
            BuildCurrent();
            Logger.Information("Initialized");
        }

        void IOrchardHost.ReloadExtensions()
        {
            DisposeShellContext();
        }

        void IOrchardHost.BeginRequest()
        {
            Logger.Debug("BeginRequest");
            BeginRequest();
        }

        void IOrchardHost.EndRequest()
        {
            Logger.Debug("EndRequest");
            EndRequest();
        }

        IWorkContextScope IOrchardHost.CreateStandaloneEnvironment(ShellSettings settings)
        {
            Logger.Debug("Creating standalone environment.");
            MonitorExtensions();
            BuildCurrent();
            var shellContext = CreateShellContext(settings);
            var workContext = shellContext.LifetimeScope.CreateWorkContextScope();
            return new StandaloneEnvironmentWorkContextScopeWrapper(workContext, shellContext);
        }

        /// <summary>
        /// Ensures shells are activated, or re-activated if extensions have changed
        /// </summary>
        IEnumerable<ShellContext> BuildCurrent()
        {
            if (_shellContexts == null)
            {
                lock (_syncLock)
                {
                    if (_shellContexts == null)
                    {
                        SetupExtensions();
                        MonitorExtensions();
                        CreateAndActivateShells();
                    }
                }
            }

            return _shellContexts;
        }

        void CreateAndActivateShells()
        {
            Logger.Information("Start creation of shells");

            // is there any tenant right now ?
            var settings = _shellSettingsManager.LoadSettings();

            //activate shell
            if (settings != null && !string.IsNullOrEmpty(settings.DataConnectionString))
            {
                var context = CreateShellContext(settings);
                ActivateShell(context);
            }
            // no settings, run the Setup
            else
            {
                var setupContext = CreateSetupContext();
                ActivateShell(setupContext);
            }

            Logger.Information("Done creating shells");
        }


        /// <summary>
        /// Starts a Shell and registers its settings in RunningShellTable
        /// </summary>
        private void ActivateShell(ShellContext context)
        {
            Logger.Debug("Activating shell context");
            context.Shell.Activate();

            lock (_shellContextsWriteLock)
            {
                _shellContexts = (_shellContexts ?? Enumerable.Empty<ShellContext>())
                                 .Concat(new[] { context })
                                .ToArray();
            }

        }

        /// <summary>
        /// Creates a shell context based on shell settings
        /// </summary>
        private ShellContext CreateShellContext(ShellSettings settings)
        {
            Logger.Debug("Creating shell context");
            return _shellContextFactory.CreateShellContext(settings);
        }

        private ShellContext CreateSetupContext()
        {
            Logger.Debug("Creating shell context for root setup");
            return _shellContextFactory.CreateSetupContext(new ShellSettings());
        }

        private void SetupExtensions()
        {
            _extensionLoaderCoordinator.SetupExtensions();
        }

        private void MonitorExtensions()
        {
            // This is a "fake" cache entry to allow the extension loader coordinator
            // notify us (by resetting _current to "null") when an extension has changed
            // on disk, and we need to reload new/updated extensions.
            _cacheManager.Get("OrchardHost_Extensions",
                              ctx =>
                              {
                                  _extensionMonitoringCoordinator.MonitorExtensions(ctx.Monitor);
                                  _hostLocalRestart.Monitor(ctx.Monitor);
                                  DisposeShellContext();
                                  return "";
                              });


        }

        /// <summary>
        /// Terminates all active shell contexts, and dispose their scope, forcing
        /// them to be reloaded if necessary.
        /// </summary>
        private void DisposeShellContext()
        {
            Logger.Information("Disposing active shell contexts");

            if (_shellContexts != null)
            {
                lock (_syncLock)
                {
                    if (_shellContexts != null)
                    {
                        foreach (var shellContext in _shellContexts)
                        {
                            shellContext.Shell.Terminate();
                            shellContext.Dispose();
                        }
                    }
                }
                _shellContexts = null;
            }
        }

        protected virtual void BeginRequest()
        {
            // Ensure all shell contexts are loaded, or need to be reloaded if
            // extensions have changed
            MonitorExtensions();
            BuildCurrent();
            StartUpdatedShells();
        }

        protected virtual void EndRequest()
        {
            // Synchronously process all pending tasks. It's safe to do this at this point
            // of the pipeline, as the request transaction has been closed, so creating a new
            // environment and transaction for these tasks will behave as expected.)
            while (_processingEngine.AreTasksPending())
            {
                Logger.Debug("Processing pending task");
                _processingEngine.ExecuteNextTask();
            }

            StartUpdatedShells();
        }

        void StartUpdatedShells()
        {
            while (_tenantsToRestart.GetState().Any())
            {
                var settings = _tenantsToRestart.GetState().First();
                _tenantsToRestart.GetState().Remove(settings);
                Logger.Debug("Updating site");
                lock (_syncLock)
                {
                    ActivateShell(settings);
                }
            }
        }

        public void ActivateShell(ShellSettings settings)
        {
            Logger.Debug("Activating shell");
            var shellContext = _shellContexts.FirstOrDefault();

            // is this is a new tenant ? or is it a tenant waiting for setup ?
            if (shellContext == null)
            {
                // create the Shell
                var context = CreateShellContext(settings);

                // activate the Shell
                ActivateShell(context);
            }
            // reload the shell as its settings have changed
            else
            {
                // dispose previous context
                shellContext.Shell.Terminate();

                //原先的LifetimeScope已被Dispose了，但是系统还是在使用原来的LifetimeScope，并没有用新生成的

                var context = _shellContextFactory.CreateShellContext(settings);
                _shellContexts = new[] { context };
                //_shellContexts = _shellContexts
                //    .Union(new[] { context }).ToArray();

                shellContext.Dispose();
                context.Shell.Activate();
            }
        }


        /// <summary>
        /// A feature is enabled/disabled,site needs to be restarted
        /// </summary>
        void IShellDescriptorManagerEventHandler.Changed(ShellDescriptor descriptor)
        {
            if (_shellContexts == null)
                return;

            if (_tenantsToRestart.GetState().Any())
                return;

            var context = _shellContexts.FirstOrDefault(x => x.Settings != null);
            if (context == null)
                return;

            if (string.IsNullOrEmpty(context.Settings.DataConnectionString))
                return;

            Logger.Debug("Shell changed, ready to restart. ");
            _tenantsToRestart.GetState().Add(context.Settings);
        }

        void IShellSettingsManagerEventHandler.Saved(ShellSettings settings)
        {
            Logger.Debug("Adding to restart");
            _tenantsToRestart.GetState().Add(settings);
        }

        // To be used from CreateStandaloneEnvironment(), also disposes the ShellContext LifetimeScope.
        private class StandaloneEnvironmentWorkContextScopeWrapper : IWorkContextScope
        {
            private readonly ShellContext _shellContext;
            private readonly IWorkContextScope _workContextScope;

            public WorkContext WorkContext
            {
                get { return _workContextScope.WorkContext; }
            }

            public StandaloneEnvironmentWorkContextScopeWrapper(IWorkContextScope workContextScope, ShellContext shellContext)
            {
                _workContextScope = workContextScope;
                _shellContext = shellContext;
            }

            public TService Resolve<TService>()
            {
                return _workContextScope.Resolve<TService>();
            }

            public bool TryResolve<TService>(out TService service)
            {
                return _workContextScope.TryResolve<TService>(out service);
            }

            public void Dispose()
            {
                _workContextScope.Dispose();
                _shellContext.Dispose();
            }
        }

    }

}
