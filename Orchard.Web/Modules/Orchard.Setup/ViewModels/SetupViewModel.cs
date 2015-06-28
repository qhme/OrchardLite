using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Orchard.Setup.ViewModels
{
    public class SetupViewModel
    {
        public SetupViewModel()
        {
        }

        public string SiteName { get; set; }


        [Required(ErrorMessage = "Username required")]
        public string AdminUsername { get; set; }


        [Required(ErrorMessage = "Password required")]
        public string AdminPassword { get; set; }


        [Compare("AdminPassword", ErrorMessage = "Password do not match")]
        public string ConfirmPassword { get; set; }


        public SetupDatabaseType DatabaseProvider { get; set; }

        public string DatabaseConnectionString { get; set; }
        public bool DatabaseIsPreconfigured { get; set; }
    }

}