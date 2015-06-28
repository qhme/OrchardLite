using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.Conventions
{
    public class CascadeAllDeleteOrphanAttribute : Attribute
    {
    }

    public class CascadeAllDeleteOrphanConvention :
     AttributeCollectionConvention<CascadeAllDeleteOrphanAttribute>
    {

        protected override void Apply(CascadeAllDeleteOrphanAttribute attribute, ICollectionInstance instance)
        {
            instance.Cascade.AllDeleteOrphan();
        }
    }

}
