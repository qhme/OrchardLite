using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Orchard.Data.Migration;
using Orchard.Environment;

namespace Orchard.Core.Dashboard.Controllers
{
    public class AdminController : Controller
    {
        private readonly IDataMigrationManager _migrationManager;
        private readonly ILifetimeScope _scope;
        public AdminController(IDataMigrationManager migrationManager,
            ILifetimeScope scope)
        {
            _migrationManager = migrationManager;
            _scope = scope;
        }
        public ActionResult Index()
        {
            return View();
        }
    }
}
