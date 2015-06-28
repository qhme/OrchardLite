using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Users.ViewModels;

namespace Orchard.Users.Validators
{
    public class UserEditValidator : AbstractValidator<UserEditViewModel>
    {
        public UserEditValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Please input the username.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("You must specify a valid email address.");
        }
    }
}