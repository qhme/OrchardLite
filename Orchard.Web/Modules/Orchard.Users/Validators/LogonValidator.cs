using FluentValidation;
using Orchard.Users.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.Users.Validators
{
    public class LogonValidator : AbstractValidator<LogonViewModel>
    {
        public LogonValidator()
        {
            RuleFor(x => x.UserNameOrEmail).NotEmpty().WithMessage("Please input username or email");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Please input password");
        }
    }
}