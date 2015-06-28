using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Environment;
using Orchard.Environment.Features;
using Orchard.Infrastructure;
using Orchard.Logging;

namespace Orchard.Data.Migration
{
    /// <summary>
    /// In order to run migrations automatically
    /// Todo 修改这个模块
    /// </summary>
    public class AutomaticDataMigrations : IOrchardShellEvents
    {
        private readonly IDataMigrationManager _dataMigrationManager;
        private readonly IFeatureManager _featureManager;

        public AutomaticDataMigrations(
            IDataMigrationManager dataMigrationManager,
            IFeatureManager featureManager)
        {
            _dataMigrationManager = dataMigrationManager;
            _featureManager = featureManager;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void Activated()
        {
            // Let's make sure that the basic set of features is enabled.  If there are any that are not enabled, then let's enable them first.
            var theseFeaturesShouldAlwaysBeActive = new[] {
                 "Dashboard", "Scheduling", "Settings"
            };

            var enabledFeatures = _featureManager.GetEnabledFeatures().Select(f => f.Id).ToList();
            var featuresToEnable = theseFeaturesShouldAlwaysBeActive.Where(shouldBeActive => !enabledFeatures.Contains(shouldBeActive)).ToList();
            if (featuresToEnable.Any())
            {
                _featureManager.EnableFeatures(featuresToEnable, true);
            }

            foreach (var feature in _dataMigrationManager.GetFeaturesThatNeedUpdate())
            {
                try
                {
                    _dataMigrationManager.Update(feature);
                }
                catch (Exception e)
                {
                    Logger.Error("Could not run migrations automatically on " + feature, e);
                }
            }
        }

        public void Terminating()
        {

        }
    }
}
