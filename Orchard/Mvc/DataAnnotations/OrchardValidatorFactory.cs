using Autofac;
using FluentValidation;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Mvc.DataAnnotations
{
    /// <summary>
    /// 解决验证类的Ioc
    /// </summary>
    public class OrchardValidatorFactory : AttributedValidatorFactory
    {
        private readonly IContainer _container;
        public OrchardValidatorFactory(IContainer container)
        {
            _container = container;
        }

        public override IValidator GetValidator(Type type)
        {
            if (type != null)
            {
                var attribute = (ValidatorAttribute)Attribute.GetCustomAttribute(type, typeof(ValidatorAttribute));
                if ((attribute != null) && (attribute.ValidatorType != null))
                {
                    var instance = ResolveUnregistered(attribute.ValidatorType);
                    return instance as IValidator;
                }
            }

            return null;
        }

        public virtual object ResolveUnregistered(Type type)
        {
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                try
                {
                    var parameters = constructor.GetParameters();
                    var parameterInstances = new List<object>();
                    foreach (var parameter in parameters)
                    {
                        var service = _container.Resolve(parameter.ParameterType);
                        if (service == null) throw new CoreException("Unkown dependency");
                        parameterInstances.Add(service);
                    }
                    return Activator.CreateInstance(type, parameterInstances.ToArray());
                }
                catch (CoreException)
                {

                }
            }

            throw new CoreException("No contructor was found that had all the dependencies satisfied.");
        }

    }
}
