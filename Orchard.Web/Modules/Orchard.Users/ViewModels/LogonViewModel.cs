using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Orchard.Users.Validators;

namespace Orchard.Users.ViewModels
{
    [Validator(typeof(LogonValidator))]
    public class LogonViewModel
    {
        public string UserNameOrEmail { get; set; }

        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public bool RememberMe { get; set; }

        [UIHint("HtmlText")]
        public string Remark { get; set; }
    }
}