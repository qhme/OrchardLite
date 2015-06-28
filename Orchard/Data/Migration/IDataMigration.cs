using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Environment.Extensions.Models;

namespace Orchard.Data.Migration
{
    public interface IDataMigration : IDependency
    {
        Feature Feature { get; }
    }
}
