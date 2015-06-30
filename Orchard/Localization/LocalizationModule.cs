using Autofac;
using Autofac.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Module = Autofac.Module;

namespace Orchard.Localization
{
    public class LocalizationModule : Module
    {
        private readonly ConcurrentDictionary<string, Localizer> _localizerCache;

        public LocalizationModule()
        {
            _localizerCache = new ConcurrentDictionary<string, Localizer>();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Text>().As<IText>().InstancePerDependency();
        }

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {

            var userProperty = FindUserProperty(registration.Activator.LimitType);

            if (userProperty != null)
            {
                var scope = registration.Activator.LimitType.FullName;

                registration.Activated += (sender, e) =>
                {
                    if (e.Instance.GetType().FullName != scope)
                    {
                        return;
                    }

                    var localizer = _localizerCache.GetOrAdd(scope, key => LocalizationUtilities.Resolve(e.Context, scope));
                    userProperty.SetValue(e.Instance, localizer, null);
                };
            }
        }

        private static PropertyInfo FindUserProperty(Type type)
        {
            return type.GetProperty("T", typeof(Localizer));
        }
    }

}
